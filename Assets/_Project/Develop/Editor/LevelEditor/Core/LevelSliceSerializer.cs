using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Формат срезов: ключи в camelCase, enum строками, числа с точкой и не больше трёх знаков после неё,
    // переводы строк CRLF, как в .editorconfig. При разборе неизвестный ключ — ошибка: опечатка не должна
    // молча превращаться в значение по умолчанию.
    public static class LevelSliceSerializer
    {
        private const string LINE_BREAK = "\r\n";
        private const int MIN_TOWER_LEVEL = 1;
        private const int MAX_TOWER_LEVEL = 5;

        // Эффективность башни — урон в секунду на столько золота полной цены.
        private const float EFFICIENCY_GOLD_AMOUNT = 100f;

        private static readonly JsonSerializerSettings _settings = CreateSettings();
        private static readonly JsonSerializer _serializer = JsonSerializer.Create(_settings);

        public static LevelSlice CreateSlice(
            LevelConfig config,
            int? campaignNumber,
            LevelPrefabInfo prefab,
            LevelStats stats,
            IReadOnlyList<LevelIssue> issues)
        {
            LevelSlice slice = CreateDataSlice(config);

            slice.CampaignNumber = campaignNumber;
            slice.PrefabPath = config.PrefabPath;
            slice.GameMode = config.GameMode.ToString();
            slice.TowerSlotsCount = prefab.Status == LevelPrefabStatus.Found ? prefab.TowerSlotsCount : null;
            slice.Roads = JToken.FromObject(stats.Roads, _serializer);
            slice.Analysis = JToken.FromObject(stats, _serializer);

            for (int i = 0; i < slice.Stages.Count; i++)
                AttachStageAnalysis(slice.Stages[i], stats.Stages[i]);

            foreach (LevelIssue issue in issues)
            {
                if (issue.IsError)
                    slice.Errors.Add(issue.ToString());
                else
                    slice.Warnings.Add(issue.ToString());
            }

            return slice;
        }

        // Только импортируемые данные уровня — для сравнения с импортируемым срезом.
        public static LevelSlice CreateDataSlice(LevelConfig config)
        {
            LevelSlice slice = new()
            {
                Level = config.name,
                StartGold = config.StartGold,
                StartPlayerHealth = config.StartPlayerHealth,
            };

            foreach (EnemiesWavesStageConfig stage in config.Stages)
                slice.Stages.Add(CreateStageSlice(stage));

            return slice;
        }

        public static BalanceReference CreateReference(
            GameConfig gameConfig,
            IReadOnlyList<CharacterConfig> enemies,
            IReadOnlyList<ShootingTowerConfig> towers,
            TowersListConfig towersList)
        {
            BalanceReference reference = new();

            if (gameConfig != null)
                reference.Game = CreateGameReference(gameConfig);

            foreach (CharacterConfig enemy in enemies)
            {
                if (enemy is MeleeConfig meleeEnemy)
                    reference.Enemies.Add(CreateEnemyReference(meleeEnemy));
            }

            Dictionary<ShootingTowerConfig, TowerUpgradeStep> upgradeSteps = MapUpgradeSteps(towersList);

            reference.Towers = towers
                .Select(tower => CreateTowerReference(tower, upgradeSteps))
                .OrderBy(tower => tower.TowerType)
                .ThenBy(tower => tower.Level ?? int.MaxValue)
                .ThenBy(tower => tower.Asset, StringComparer.Ordinal)
                .ToList();

            return reference;
        }

        public static string ToJson(object value)
        {
            StringBuilder builder = new();

            using (StringWriter writer = new(builder, CultureInfo.InvariantCulture))
            {
                writer.NewLine = LINE_BREAK;
                _serializer.Serialize(writer, value);
            }

            builder.Append(LINE_BREAK);

            return builder.ToString();
        }

        // Срезы стейджа и волны с блоком analysis — для буфера обмена окна редактора.
        public static StageSlice CreateStageSlice(EnemiesWavesStageConfig stage, StageStats stats)
        {
            StageSlice stageSlice = CreateStageSlice(stage);
            AttachStageAnalysis(stageSlice, stats);

            return stageSlice;
        }

        public static WaveSlice CreateWaveSlice(EnemiesWaveConfig wave, WaveStats stats)
        {
            WaveSlice waveSlice = CreateWaveSlice(wave);
            waveSlice.Analysis = JToken.FromObject(stats, _serializer);

            return waveSlice;
        }

        public static LevelSlice ParseSlice(string json) => JsonConvert.DeserializeObject<LevelSlice>(json, _settings);

        // JSON из буфера обмена разбирается в два шага: сначала дерево — по его ключам видно, уровень это, стейдж
        // или волна, — затем срез нужного типа. Даты не распознаются: заметки остаются строками как есть.
        public static JToken ParseToken(string json)
        {
            using StringReader stringReader = new(json);
            using JsonTextReader reader = new(stringReader)
            {
                DateParseHandling = DateParseHandling.None,
            };

            JToken token = JToken.ReadFrom(reader);

            if (reader.Read())
                throw new JsonReaderException("После JSON есть лишний текст");

            return token;
        }

        public static T FromToken<T>(JToken token) => token.ToObject<T>(_serializer);

        private static JsonSerializerSettings CreateSettings()
        {
            DefaultContractResolver contractResolver = new()
            {
                // Ключи словарей — имена ассетов врагов, регистр в них сохраняем.
                NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = false },
            };

            return new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture,
                DateParseHandling = DateParseHandling.None,
                MissingMemberHandling = MissingMemberHandling.Error,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { AllowIntegerValues = false },
                    new RoundingJsonConverter(LevelSliceNumbers.DECIMALS),
                },
            };
        }

        private static void AttachStageAnalysis(StageSlice stageSlice, StageStats stageStats)
        {
            stageSlice.Analysis = JToken.FromObject(stageStats, _serializer);

            for (int i = 0; i < stageSlice.Waves.Count; i++)
                stageSlice.Waves[i].Analysis = JToken.FromObject(stageStats.Waves[i], _serializer);
        }

        private static StageSlice CreateStageSlice(EnemiesWavesStageConfig stage)
        {
            StageSlice stageSlice = new()
            {
                StageTime = stage.StageTime,
                TimeToSkipStage = stage.TimeToSkipStage,
                Note = stage.Note ?? string.Empty,
            };

            foreach (EnemiesWaveConfig wave in LevelStatsCalculator.GetWaves(stage))
                stageSlice.Waves.Add(CreateWaveSlice(wave));

            return stageSlice;
        }

        private static WaveSlice CreateWaveSlice(EnemiesWaveConfig wave)
        {
            WaveSlice waveSlice = new()
            {
                PatternRepeats = wave.PatternRepeats,
                PathNumber = wave.PathNumber,
                Lane = wave.Lane,
                StartDelay = wave.StartDelay,
                DelayBetweenSpawns = wave.DelayBetweenSpawns,
                Note = wave.Note ?? string.Empty,
            };

            foreach (WaveEnemyGroup group in LevelStatsCalculator.GetGroups(wave))
            {
                waveSlice.Groups.Add(new EnemyGroupSlice
                {
                    Enemy = group.EnemyConfig != null ? group.EnemyConfig.name : null,
                    Count = group.Count,
                });
            }

            return waveSlice;
        }

        private static GameReference CreateGameReference(GameConfig gameConfig)
        {
            return new GameReference
            {
                GoldIndexForRemainingStageSeconds = gameConfig.GoldIndexForRemainingStageSeconds,
                MaxGoldForSkipStage = gameConfig.MaxGoldForSkipStage,
                SellTowerGoldReturnIndex = gameConfig.SellTowerGoldReturnIndex,
                GoldRewardByAdvert = gameConfig.GoldRewardByAdvert,
            };
        }

        private static EnemyReference CreateEnemyReference(MeleeConfig enemy)
        {
            return new EnemyReference
            {
                Asset = enemy.name,
                Name = enemy.CharacterName,
                MaxHealth = enemy.MaxHealth,
                MoveSpeed = enemy.MoveSpeed,
                DamageResistanceType = enemy.DamageResistanceType,
                DamageResistanceIndex = enemy.DamageResistanceIndex,
                EffectiveHealthPhysic = LevelStatsCalculator.CalculateEffectiveHealth(enemy, DamageTypes.Physic),
                EffectiveHealthMagic = LevelStatsCalculator.CalculateEffectiveHealth(enemy, DamageTypes.Magic),
                GoldOnDeath = enemy.GoldOnDeath,
                DamageOnFinishPath = enemy.DamageOnFinishPath,
                IsBoss = enemy.IsBoss,
            };
        }

        private static TowerReference CreateTowerReference(
            ShootingTowerConfig tower,
            IReadOnlyDictionary<ShootingTowerConfig, TowerUpgradeStep> upgradeSteps)
        {
            float damagePerSecond = tower.AttackDamage * tower.AttackPerSecond;

            TowerReference reference = new()
            {
                Asset = tower.name,
                TowerType = tower.TowerType,
                Name = tower.TowerName,
                Cost = tower.Cost,
                AttackDamage = tower.AttackDamage,
                DamageType = tower.DamageType,
                AttackPerSecond = tower.AttackPerSecond,
                DamagePerSecond = damagePerSecond,
                AttackRange = tower.AttackRange,
            };

            if (upgradeSteps.TryGetValue(tower, out TowerUpgradeStep upgradeStep))
            {
                reference.Level = upgradeStep.Level;
                reference.TotalCost = upgradeStep.TotalCost;

                if (upgradeStep.TotalCost > 0)
                    reference.DamagePerSecondPer100Gold = damagePerSecond / upgradeStep.TotalCost * EFFICIENCY_GOLD_AMOUNT;
            }

            return reference;
        }

        // Уровни и полная цена башен — как в игре: TowersListConfig.GetBy(тип, уровень), а улучшение всегда ведёт
        // на следующий уровень. Если конфига какого-то уровня нет, следующие уровни этого типа недостижимы.
        private static Dictionary<ShootingTowerConfig, TowerUpgradeStep> MapUpgradeSteps(TowersListConfig towersList)
        {
            Dictionary<ShootingTowerConfig, TowerUpgradeStep> upgradeSteps = new();

            if (towersList == null)
                return upgradeSteps;

            foreach (TowerTypes towerType in Enum.GetValues(typeof(TowerTypes)))
            {
                int totalCost = 0;

                for (int level = MIN_TOWER_LEVEL; level <= MAX_TOWER_LEVEL; level++)
                {
                    ShootingTowerConfig tower = FindTower(towersList, towerType, level);

                    if (tower == null)
                        break;

                    totalCost += tower.Cost;
                    upgradeSteps.TryAdd(tower, new TowerUpgradeStep(level, totalCost));
                }
            }

            return upgradeSteps;
        }

        // GetBy бросает исключение, если для типа нет дерева улучшений или уровень не поддерживается.
        private static ShootingTowerConfig FindTower(TowersListConfig towersList, TowerTypes towerType, int level)
        {
            try
            {
                return towersList.GetBy(towerType, level);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private readonly struct TowerUpgradeStep
        {
            public TowerUpgradeStep(int level, int totalCost)
            {
                Level = level;
                TotalCost = totalCost;
            }

            public int Level { get; }
            public int TotalCost { get; }
        }
    }
}
