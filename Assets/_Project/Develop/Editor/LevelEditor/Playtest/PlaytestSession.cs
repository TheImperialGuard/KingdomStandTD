using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Плейтест на одну игровую сессию. Точка входа забирает отсюда аргументы и открывает уровень вместо меню, а каждый
    // прогон — и «Заново» в попапах — получает свежую копию уровня и свой лог; события игры сессия передаёт логу
    // идущего прогона. Сессия кончается, когда загружается главное меню: дальше уровни из меню играются как обычно.
    public class PlaytestSession : IPlaytestSession, IDisposable
    {
        private const int NON_CAMPAIGN_LEVEL_NUMBER = 0;

        public event Action Ended;

        private readonly int? _campaignNumber;

        private LevelConfig _levelCopy;
        private PlaytestLogService _log;
        private bool _isStartArgsTaken;

        public PlaytestSession(PlaytestLaunch launch, int? campaignNumber)
        {
            Launch = launch;
            _campaignNumber = campaignNumber;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public PlaytestLaunch Launch { get; }

        public int CurrentStageNumber => _log?.CurrentStageNumber ?? PlaytestLogService.NOT_STARTED_STAGE_NUMBER;

        // Номер уровня игре нужен только для сообщений в консоли: результат плейтеста не сохраняется.
        public GameplayInputArgs TakeStartArgs()
        {
            if (_isStartArgsTaken)
                return null;

            _isStartArgsTaken = true;

            return new GameplayInputArgs(_campaignNumber ?? NON_CAMPAIGN_LEVEL_NUMBER);
        }

        // Прошлая копия к этому моменту не нужна: сцена Gameplay перезагружается целиком, вместе со своим контейнером.
        public LevelConfig CreateLevelConfig()
        {
            DestroyLevelCopy();

            _levelCopy = PlaytestLevelCopy.Create(Launch);

            return _levelCopy;
        }

        public void ProcessRegistrations(DIContainer container) => container.RegisterAsSingle(CreateLog).NonLazy();

        public void OnGameEnded(LevelResults results) => _log?.OnGameEnded(results);

        public void OnEnemySpawned(Entity enemy, CharacterConfig enemyConfig, EnemiesWaveConfig wave)
            => _log?.OnEnemySpawned(enemy, enemyConfig, wave);

        public void OnTowerBought(TowerTypes towerType, int level, int cost) => _log?.OnTowerBought(towerType, level, cost);

        public void OnTowerSold(TowerTypes towerType, int level, int gold) => _log?.OnTowerSold(towerType, level, gold);

        public void OnAbilityBought(TowerTypes towerType, AbilityConfig ability, int cost)
            => _log?.OnAbilityBought(towerType, ability, cost);

        public void OnSkipGoldEarned(int gold) => _log?.OnSkipGoldEarned(gold);

        // Stop не освобождает сцену, поэтому идущий прогон дописывается здесь, по выходу из Play Mode.
        public void CompleteRun() => _log?.CompleteInterrupted();

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            DestroyLevelCopy();
        }

        private PlaytestLogService CreateLog(DIContainer container)
        {
            _log = new PlaytestLogService(
                container.Resolve<StageProviderService>(),
                container.Resolve<WalletService>(),
                container.Resolve<PlayerHealth>(),
                container.Resolve<EntitiesLifeContext>(),
                Launch,
                _levelCopy,
                _campaignNumber);

            return _log;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == Scenes.MainMenu)
                Ended?.Invoke();
        }

        private void DestroyLevelCopy()
        {
            if (_levelCopy != null)
                Object.DestroyImmediate(_levelCopy);

            _levelCopy = null;
        }
    }
}
