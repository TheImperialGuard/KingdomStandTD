using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets._Project.Develop.Editor.EndlessMode;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Лог одного прогона плейтеста (что в нём — см. PlaytestLog). Живёт в контейнере сцены Gameplay: регистрирует его
    // PlaytestSession через хук, от неё же приходят события игры. Файл переписывается на каждом стейдже, в конце игры,
    // при уходе со сцены и при выходе из Play Mode: Stop сцену не освобождает, и без этого оборванный прогон пропал бы.
    // Прогон, не дошедший до первого стейджа, не пишется. Стейджи прогона — из источника стейджей сцены: у кампании это
    // копия уровня без стейджей до стартового, у бесконечного — волны генератора с их настоящими номерами.
    public class PlaytestLogService : IInitializable, IDisposable
    {
        public const int NOT_STARTED_STAGE_NUMBER = 0;

        private const string LOG_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const int FIRST_TOWER_LEVEL = 1;
        private const int UNKNOWN_WAVE_NUMBER = 0;
        private const int NO_DAMAGE = 0;
        private const int ENDLESS_SNAPSHOT_WAVES_COUNT = 0;

        private readonly StageProviderService _stageProvider;
        private readonly IStagesSource _stagesSource;
        private readonly WalletService _wallet;
        private readonly PlayerHealth _playerHealth;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly PlaytestLaunch _launch;
        private readonly LevelConfig _levelCopy;
        private readonly int? _campaignNumber;
        private readonly EndlessModeConfig _endlessConfig;

        private readonly Dictionary<int, PlaytestStageLog> _stagesByNumber = new();
        private readonly Dictionary<EnemiesWaveConfig, WaveAddress> _waveAddresses = new();
        private readonly Dictionary<Entity, TrackedEnemy> _enemies = new();
        private readonly Dictionary<(TowerTypes Tower, int Level), int> _standingTowers = new();
        private readonly List<IDisposable> _disposables = new();

        private PlaytestLog _log;
        private string _filePath;
        private int _stageOffset;
        private EnemiesOnFieldCounter _enemiesCounter;

        private PlaytestStageLog _currentStage;
        private PlaytestStageLog _lastClosedStage;
        private bool _hasStarted;

        private float _runStartTime;
        private float _runStartRealTime;
        private float _endTime;
        private int? _endFrame;
        private bool _isCompleted;

        // endlessConfig — конфиг бесконечного режима; null — играется уровень кампании
        public PlaytestLogService(
            StageProviderService stageProvider,
            IStagesSource stagesSource,
            WalletService wallet,
            PlayerHealth playerHealth,
            EntitiesLifeContext entitiesLifeContext,
            PlaytestLaunch launch,
            LevelConfig levelCopy,
            int? campaignNumber,
            EndlessModeConfig endlessConfig)
        {
            _stageProvider = stageProvider;
            _stagesSource = stagesSource;
            _wallet = wallet;
            _playerHealth = playerHealth;
            _entitiesLifeContext = entitiesLifeContext;
            _launch = launch;
            _levelCopy = levelCopy;
            _campaignNumber = campaignNumber;
            _endlessConfig = endlessConfig;
        }

        // Настоящий номер идущего стейджа; NOT_STARTED_STAGE_NUMBER — первый стейдж прогона ещё не начался.
        public int CurrentStageNumber => _hasStarted ? _currentStage.Stage : NOT_STARTED_STAGE_NUMBER;

        private float GameTime => Time.time - _runStartTime;

        private int Gold => _wallet.GetCurrency(CurrencyTypes.Gold).Value;

        // После кадра, в котором игра закончилась, враги ещё ходят, но прогон уже окончен — события в лог не идут.
        private bool IsAfterEnd => _isCompleted || (_endFrame.HasValue && Time.frameCount > _endFrame.Value);

        public void Initialize()
        {
            DateTime startedAt = DateTime.Now;

            _runStartTime = Time.time;
            _runStartRealTime = Time.realtimeSinceStartup;
            _filePath = PlaytestLogFile.CreatePath(_launch.Level.name, startedAt);

            // В копии кампании нет стейджей до стартового: столько номеров и сдвигается. У бесконечного уровня сдвига нет —
            // источник сам начинает с волны N.
            _stageOffset = _launch.Level.Stages.Count - _levelCopy.Stages.Count;

            // Номер растёт при запуске стейджа: первым запустится следующий за текущим
            int firstRunStageNumber = _stageProvider.CurrentStageNumber.Value + 1;

            _log = new PlaytestLog
            {
                Level = _launch.Level.name,
                CampaignNumber = _campaignNumber,
                GameMode = _launch.Level.GameMode.ToString(),
                StartedAt = startedAt.ToString(LOG_TIME_FORMAT, CultureInfo.InvariantCulture),
                StartStage = ToLevelStageNumber(firstRunStageNumber),
                LevelStagesCount = _launch.IsEndless ? null : _launch.Level.Stages.Count,
                StartGold = Gold,
                StartPlayerHealth = _playerHealth.Current.Value,
                HasUnsavedChanges = HasUnsavedChanges(),
                ForcedWeather = _launch.IsEndless ? EndlessModeEditorSettings.ForcedWeather : null,
                Result = PlaytestResult.InProgress,
                LevelSnapshot = CreateLevelSnapshot(),
            };

            _enemiesCounter = new EnemiesOnFieldCounter(_entitiesLifeContext);
            _currentStage = OpenStage(ToLevelStageNumber(firstRunStageNumber));

            _disposables.Add(_stageProvider.CurrentStageNumber.Subscribe(OnRunStageNumberChanged));
            _disposables.Add(_playerHealth.Current.Subscribe(OnPlayerHealthChanged));
            _disposables.Add(_enemiesCounter.EnemiesCount.Subscribe(OnEnemiesCountChanged));
            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public void OnGameEnded(LevelResults results)
        {
            if (_endFrame.HasValue)
                return;

            _log.Result = ToPlaytestResult(results);
            MarkEnd();

            // Добивающий прорыв доходит до лога уже после сообщения о конце игры, в том же кадре, — итог пишется позже.
            EditorApplication.delayCall += Complete;
        }

        public void OnEnemySpawned(Entity enemy, CharacterConfig enemyConfig, EnemiesWaveConfig wave)
        {
            if (IsAfterEnd)
                return;

            WaveAddress address = FindWaveAddress(wave);
            PlaytestStageLog stage = GetOrCreateStage(address.StageNumber);

            TrackedEnemy tracked = new(
                GetOrCreateEnemyLog(stage, enemyConfig.name),
                GetOrCreateWaveLog(stage, address.WaveNumber));

            // Множитель здоровья бесконечного режима враг получает раньше, по событию спавна
            tracked.Enemy.MaxHealth = enemy.MaxHealth.Value;
            tracked.Enemy.Spawned++;
            tracked.Wave.Spawned++;
            tracked.DeathSubscription = enemy.IsDead.Subscribe((_, isDead) => OnEnemyDeathChanged(enemy, tracked, isDead));

            _enemies[enemy] = tracked;
        }

        public void OnTowerBought(TowerTypes towerType, int level, int cost)
        {
            if (IsAfterEnd)
                return;

            _currentStage.Gold.TowerPurchases += cost;
            _currentStage.Towers.Bought.Add(new PlaytestTowerDeal { Tower = towerType, Level = level, Gold = cost });

            if (level > FIRST_TOWER_LEVEL)
                RemoveStandingTower(towerType, level - 1);

            AddStandingTower(towerType, level);
        }

        public void OnTowerSold(TowerTypes towerType, int level, int gold)
        {
            if (IsAfterEnd)
                return;

            _currentStage.Gold.Sales += gold;
            _currentStage.Towers.Sold.Add(new PlaytestTowerDeal { Tower = towerType, Level = level, Gold = gold });

            RemoveStandingTower(towerType, level);
        }

        public void OnAbilityBought(TowerTypes towerType, AbilityConfig ability, int cost)
        {
            if (IsAfterEnd)
                return;

            _currentStage.Gold.AbilityPurchases += cost;
            _currentStage.Towers.Abilities.Add(new PlaytestAbilityPurchase { Tower = towerType, Ability = ability.Name, Gold = cost });
        }

        // Золото за скип приходит сразу после смены стейджа, в том же вызове: оно — доход закрытого стейджа и часть
        // золота на старте следующего.
        public void OnSkipGoldEarned(int gold)
        {
            if (IsAfterEnd || _lastClosedStage == null)
                return;

            _lastClosedStage.IsSkipped = true;
            _lastClosedStage.Gold.Skip += gold;
            _lastClosedStage.Gold.AtEnd += gold;
            _currentStage.Gold.AtStart += gold;

            Write();
        }

        // Погода приходит в начале каждой волны вместе с её номером, поэтому неважно, раньше или позже открытия стейджа.
        public void OnWaveWeather(WeatherKinds weather, int waveNumber)
        {
            if (IsAfterEnd)
                return;

            GetOrCreateStage(ToLevelStageNumber(waveNumber)).Weather = weather;
        }

        public void OnLightningStruck(float damage, int hitEnemiesCount)
        {
            if (IsAfterEnd)
                return;

            PlaytestLightningLog lightning = _currentStage.Lightning ??= new PlaytestLightningLog();

            lightning.Strikes++;
            lightning.HitEnemies += hitEnemiesCount;
            lightning.Damage += damage * hitEnemiesCount;
        }

        // Прогон оборван: «Заново», выход в меню или Stop. Если игра уже закончилась, итог остаётся её итогом.
        public void CompleteInterrupted()
        {
            if (_log == null)
                return;

            if (_endFrame.HasValue == false)
            {
                _log.Result = PlaytestResult.Interrupted;
                MarkEnd();
            }

            Complete();
        }

        public void Dispose()
        {
            CompleteInterrupted();

            EditorApplication.delayCall -= Complete;
            _entitiesLifeContext.Released -= OnEntityReleased;

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();

            foreach (TrackedEnemy tracked in _enemies.Values)
                tracked.DeathSubscription.Dispose();

            _enemiesCounter?.Dispose();

            _disposables.Clear();
            _enemies.Clear();
        }

        private void OnRunStageNumberChanged(int oldNumber, int newNumber)
        {
            if (IsAfterEnd)
                return;

            float now = GameTime;

            if (_hasStarted == false)
            {
                _hasStarted = true;
                _log.PreparationTime = now;
                _currentStage.StartTime = now;

                Write();
                return;
            }

            UpdateStageEnd(_currentStage, now);
            _lastClosedStage = _currentStage;

            _currentStage = OpenStage(ToLevelStageNumber(newNumber));
            _currentStage.StartTime = now;

            Write();
        }

        private void OnPlayerHealthChanged(int oldValue, int newValue)
        {
            if (IsAfterEnd || newValue >= oldValue)
                return;

            _currentStage.Health.Lost += oldValue - newValue;
        }

        // Пик — за время стейджа, как золото и здоровье: враги прошлых стейджей на поле тоже в счёт
        private void OnEnemiesCountChanged(int oldCount, int newCount)
        {
            if (IsAfterEnd)
                return;

            _currentStage.PeakEnemies = Math.Max(_currentStage.PeakEnemies, newCount);
            _log.PeakEnemies = Math.Max(_log.PeakEnemies, newCount);
        }

        private void OnEnemyDeathChanged(Entity enemy, TrackedEnemy tracked, bool isDead)
        {
            if (isDead == false || IsAfterEnd)
                return;

            int gold = enemy.GoldOnDeath.Value;

            tracked.Enemy.Killed++;
            tracked.Enemy.KillGold += gold;
            tracked.Wave.Killed++;

            _currentStage.Gold.Kills += gold;
        }

        private void OnEntityReleased(Entity entity)
        {
            if (_enemies.TryGetValue(entity, out TrackedEnemy tracked) == false)
                return;

            _enemies.Remove(entity);
            tracked.DeathSubscription.Dispose();

            if (IsAfterEnd)
                return;

            if (entity.TryGetIsPathFinished(out ReactiveVariable<bool> isPathFinished) == false || isPathFinished.Value == false)
                return;

            int damage = entity.TryGetDamageOnFinishPath(out ReactiveVariable<int> damageOnFinishPath) ? damageOnFinishPath.Value : NO_DAMAGE;

            tracked.Enemy.Leaked++;
            tracked.Enemy.Damage += damage;
            tracked.Wave.Leaked++;
            tracked.Wave.Damage += damage;
        }

        private void MarkEnd()
        {
            _endFrame = Time.frameCount;
            _endTime = GameTime;
        }

        private void Complete()
        {
            if (_isCompleted)
                return;

            _isCompleted = true;

            if (_hasStarted == false)
                return;

            UpdateStageEnd(_currentStage, _endTime);
            UpdateTotals(_endTime);

            if (TryWriteFile())
                Debug.Log($"Лог плейтеста: {_filePath}");
        }

        private void Write()
        {
            float now = GameTime;

            UpdateStageEnd(_currentStage, now);
            UpdateTotals(now);
            TryWriteFile();
        }

        // Ошибка записи не должна ломать игру: прогон продолжается, а лог допишется при следующей записи.
        private bool TryWriteFile()
        {
            try
            {
                PlaytestLogFile.Write(_filePath, _log);
                return true;
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                Debug.LogWarning($"Лог плейтеста не записан: {exception.Message}");
                return false;
            }
        }

        private PlaytestStageLog OpenStage(int levelStageNumber)
        {
            PlaytestStageLog stage = GetOrCreateStage(levelStageNumber);

            stage.Gold.AtStart = Gold;
            stage.Health.AtStart = _playerHealth.Current.Value;
            stage.Towers.AtStart = CreateTowersSnapshot();
            stage.PeakEnemies = Math.Max(stage.PeakEnemies, _enemiesCounter.EnemiesCount.Value);

            MapWaves(levelStageNumber);

            return stage;
        }

        private void UpdateStageEnd(PlaytestStageLog stage, float time)
        {
            PlaytestGoldLog gold = stage.Gold;

            stage.Duration = time - stage.StartTime;
            gold.AtEnd = Gold;
            gold.Other = gold.AtEnd - gold.AtStart - gold.Kills - gold.Skip - gold.Sales + gold.TowerPurchases + gold.AbilityPurchases;
        }

        private void UpdateTotals(float time)
        {
            _log.GameTime = time;
            _log.RealTime = Time.realtimeSinceStartup - _runStartRealTime;
            _log.ReachedStage = _currentStage.Stage;
            _log.FinalGold = Gold;
            _log.FinalPlayerHealth = _playerHealth.Current.Value;
            _log.TowersAtEnd = CreateTowersSnapshot();
        }

        private PlaytestStageLog GetOrCreateStage(int levelStageNumber)
        {
            if (_stagesByNumber.TryGetValue(levelStageNumber, out PlaytestStageLog stage))
                return stage;

            EnemiesWavesStageConfig stageConfig = FindStageConfig(levelStageNumber);

            stage = new PlaytestStageLog
            {
                Stage = levelStageNumber,
                StageTime = stageConfig != null ? stageConfig.StageTime : 0f,
                HealthMultiplier = _endlessConfig?.HealthMultiplier.GetValue(levelStageNumber),
            };

            _stagesByNumber.Add(levelStageNumber, stage);
            _log.Stages.Add(stage);

            return stage;
        }

        // Конфиг стейджа по настоящему номеру; null — у источника такого стейджа нет
        private EnemiesWavesStageConfig FindStageConfig(int levelStageNumber)
        {
            int runStageNumber = levelStageNumber - _stageOffset;

            if (_stagesSource.Has(runStageNumber) == false)
                return null;

            return _stagesSource.Get(runStageNumber) as EnemiesWavesStageConfig;
        }

        private static PlaytestEnemyLog GetOrCreateEnemyLog(PlaytestStageLog stage, string enemyName)
        {
            PlaytestEnemyLog enemyLog = stage.Enemies.Find(log => log.Enemy == enemyName);

            if (enemyLog != null)
                return enemyLog;

            enemyLog = new PlaytestEnemyLog { Enemy = enemyName };
            stage.Enemies.Add(enemyLog);

            return enemyLog;
        }

        // Волны держатся по порядку номеров, хотя выходят в порядке своего старта.
        private static PlaytestWaveLog GetOrCreateWaveLog(PlaytestStageLog stage, int waveNumber)
        {
            PlaytestWaveLog waveLog = stage.Waves.Find(log => log.Wave == waveNumber);

            if (waveLog != null)
                return waveLog;

            waveLog = new PlaytestWaveLog { Wave = waveNumber };

            int insertIndex = stage.Waves.FindIndex(log => log.Wave > waveNumber);

            if (insertIndex < 0)
                stage.Waves.Add(waveLog);
            else
                stage.Waves.Insert(insertIndex, waveLog);

            return waveLog;
        }

        // Волны стейджа запоминаются, когда он открывается: враги стейджа выходят только после его начала, а бесконечный
        // источник собирает волну по первому запросу.
        private void MapWaves(int levelStageNumber)
        {
            EnemiesWavesStageConfig stageConfig = FindStageConfig(levelStageNumber);

            if (stageConfig == null)
                return;

            IReadOnlyList<EnemiesWaveConfig> waves = stageConfig.EnemiesWaveConfigs;

            for (int waveIndex = 0; waveIndex < waves.Count; waveIndex++)
                _waveAddresses[waves[waveIndex]] = new WaveAddress(levelStageNumber, waveIndex + 1);
        }

        // Волна не из копии уровня не ожидается; если встретится, враг числится за идущим стейджем.
        private WaveAddress FindWaveAddress(EnemiesWaveConfig wave)
        {
            if (_waveAddresses.TryGetValue(wave, out WaveAddress address))
                return address;

            return new WaveAddress(_currentStage.Stage, UNKNOWN_WAVE_NUMBER);
        }

        private int ToLevelStageNumber(int runStageNumber) => runStageNumber + _stageOffset;

        private void AddStandingTower(TowerTypes towerType, int level)
        {
            _standingTowers.TryGetValue((towerType, level), out int count);
            _standingTowers[(towerType, level)] = count + 1;
        }

        private void RemoveStandingTower(TowerTypes towerType, int level)
        {
            if (_standingTowers.TryGetValue((towerType, level), out int count) == false)
                return;

            int remainingCount = count - 1;

            if (remainingCount > 0)
                _standingTowers[(towerType, level)] = remainingCount;
            else
                _standingTowers.Remove((towerType, level));
        }

        private List<PlaytestTowerCount> CreateTowersSnapshot()
        {
            return _standingTowers
                .OrderBy(pair => pair.Key.Tower)
                .ThenBy(pair => pair.Key.Level)
                .Select(pair => new PlaytestTowerCount { Tower = pair.Key.Tower, Level = pair.Key.Level, Count = pair.Value })
                .ToList();
        }

        // У бесконечного уровня несохранённые правки могут быть и в конфигах режима и погоды
        private bool HasUnsavedChanges()
        {
            if (EditorUtility.IsDirty(_launch.Level))
                return true;

            if (_endlessConfig == null)
                return false;

            return EditorUtility.IsDirty(_endlessConfig) || (_endlessConfig.Weather != null && EditorUtility.IsDirty(_endlessConfig.Weather));
        }

        // Снимок уровня — тот же срез, что в Docs/Balance; если собрать его не вышло, лог пишется без него. Снимок
        // бесконечного уровня — без волн: из тех же настроек они собираются заново, и для 50 волн лог вырос бы впятеро.
        private JToken CreateLevelSnapshot()
        {
            try
            {
                if (_launch.IsEndless)
                {
                    LevelSlice endlessSlice = EndlessSliceSerializer.CreateSlice(
                        _launch.Level,
                        _campaignNumber,
                        LevelPrefabReader.Read(_launch.Level),
                        LevelEditorAssets.FindGameConfig(),
                        ENDLESS_SNAPSHOT_WAVES_COUNT);

                    return LevelSliceSerializer.ToToken(endlessSlice);
                }

                string sliceJson = LevelSliceExporter.CreateSliceJson(
                    _launch.Level,
                    LevelEditorAssets.FindGameConfig(),
                    LevelEditorAssets.FindLevelsList());

                return LevelSliceSerializer.ParseToken(sliceJson);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Снимок уровня для лога плейтеста не собран: {exception.Message}");
                return null;
            }
        }

        private static PlaytestResult ToPlaytestResult(LevelResults results)
        {
            switch (results)
            {
                case LevelResults.Perfect:
                    return PlaytestResult.Perfect;

                case LevelResults.Average:
                    return PlaytestResult.Average;

                case LevelResults.Bad:
                    return PlaytestResult.Bad;

                default:
                    return PlaytestResult.Defeat;
            }
        }

        private class TrackedEnemy
        {
            public TrackedEnemy(PlaytestEnemyLog enemy, PlaytestWaveLog wave)
            {
                Enemy = enemy;
                Wave = wave;
            }

            public PlaytestEnemyLog Enemy { get; }
            public PlaytestWaveLog Wave { get; }
            public IDisposable DeathSubscription { get; set; }
        }

        private readonly struct WaveAddress
        {
            public WaveAddress(int stageNumber, int waveNumber)
            {
                StageNumber = stageNumber;
                WaveNumber = waveNumber;
            }

            public int StageNumber { get; }
            public int WaveNumber { get; }
        }
    }
}
