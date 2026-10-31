using System.Collections.Generic;
using Assets._Project.Develop.Editor.LevelEditor;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.GameMode;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Проверка конфига бесконечного режима. Ошибки — то, на чём генератор волн сломается; предупреждения — то, что
    // работает, но скорее всего не задумано.
    public static class EndlessModeConfigValidator
    {
        private const string EnemiesCountTableName = "Число врагов";
        private const string DelayBetweenSpawnsTableName = "Интервал между врагами";
        private const string RestTimeTableName = "Передышка";
        private const string HealthMultiplierTableName = "Множитель здоровья";

        // levelPrefab — префаб уровня режима, загруженный по его PrefabPath; null, если загрузить не удалось
        public static List<LevelIssue> Validate(EndlessModeConfig config, Level levelPrefab)
        {
            List<LevelIssue> issues = new();

            ValidateLevel(config, levelPrefab, issues);
            ValidateTable(config.EnemiesCount, EnemiesCountTableName, issues);
            ValidateTable(config.DelayBetweenSpawns, DelayBetweenSpawnsTableName, issues);
            ValidateTable(config.RestTime, RestTimeTableName, issues);
            ValidateTable(config.HealthMultiplier, HealthMultiplierTableName, issues);
            ValidateTiers(config, issues);
            ValidateBosses(config, issues);

            return issues;
        }

        private static void ValidateLevel(EndlessModeConfig config, Level levelPrefab, List<LevelIssue> issues)
        {
            if (config.Level == null)
            {
                issues.Add(LevelIssue.Error("Не задан уровень режима (Level)"));
                return;
            }

            if (config.Level.GameMode != GameModes.Infinity)
            {
                issues.Add(LevelIssue.Warning(
                    $"У уровня {config.Level.name} режим {config.Level.GameMode}, а не {GameModes.Infinity}"));
            }

            if (config.Level.Stages.Count > 0)
                issues.Add(LevelIssue.Warning($"Стейджи уровня {config.Level.name} в бесконечном режиме не используются"));

            if (levelPrefab == null)
            {
                issues.Add(LevelIssue.Error($"Префаб уровня не найден в Resources по пути «{config.Level.PrefabPath}»"));
                return;
            }

            if (levelPrefab.RoadPaths.Count == 0)
                issues.Add(LevelIssue.Error($"На префабе уровня {levelPrefab.name} нет ни одной дороги"));
        }

        private static void ValidateTiers(EndlessModeConfig config, List<LevelIssue> issues)
        {
            if (config.Tiers.Count == 0)
            {
                issues.Add(LevelIssue.Error("Нет ни одного тира врагов"));
                return;
            }

            for (int i = 0; i < config.Tiers.Count; i++)
            {
                EndlessTier tier = config.Tiers[i];
                string tierName = GetTierName(tier, i);

                ValidateTable(tier.Share, $"Доля, {tierName}", issues);

                if (tier.Enemies.Count == 0)
                    issues.Add(LevelIssue.Warning($"{tierName}: нет врагов, его доля уходит другим тирам"));

                foreach (EndlessTierEnemy tierEnemy in tier.Enemies)
                    ValidateTierEnemy(tierEnemy, tierName, issues);
            }
        }

        private static void ValidateTierEnemy(EndlessTierEnemy tierEnemy, string tierName, List<LevelIssue> issues)
        {
            if (tierEnemy.EnemyConfig == null)
            {
                issues.Add(LevelIssue.Warning($"{tierName}: пустой элемент в списке врагов, генератор его пропустит"));
                return;
            }

            if (tierEnemy.EnemyConfig.IsBoss)
                issues.Add(LevelIssue.Warning($"{tierName}: {tierEnemy.EnemyConfig.name} — босс, он выйдет как обычный враг"));
        }

        private static void ValidateBosses(EndlessModeConfig config, List<LevelIssue> issues)
        {
            if (config.Bosses.Count == 0)
            {
                issues.Add(LevelIssue.Warning("Список боссов пуст: боссовых волн не будет"));
                return;
            }

            foreach (MeleeConfig boss in config.Bosses)
            {
                if (boss == null)
                {
                    issues.Add(LevelIssue.Warning("Пустой элемент в списке боссов: его волна выйдет обычной"));
                    continue;
                }

                if (boss.IsBoss == false)
                {
                    issues.Add(LevelIssue.Warning(
                        $"{boss.name} в списке боссов — не босс: без полоски и музыки босса, вызов волны при нём открыт"));
                }
            }
        }

        private static void ValidateTable(WaveValueTable table, string tableName, List<LevelIssue> issues)
        {
            IReadOnlyList<WaveValueKey> keys = table.Keys;

            if (keys.Count == 0)
            {
                issues.Add(LevelIssue.Error($"Таблица «{tableName}» пуста: значения по волнам брать неоткуда"));
                return;
            }

            for (int i = 1; i < keys.Count; i++)
            {
                int previousWave = keys[i - 1].Wave;
                int wave = keys[i].Wave;

                if (wave <= previousWave)
                {
                    issues.Add(LevelIssue.Error(
                        $"Таблица «{tableName}»: волны ключей должны расти, а за волной {previousWave} идёт волна {wave}"));
                    return;
                }
            }
        }

        private static string GetTierName(EndlessTier tier, int tierIndex)
        {
            if (string.IsNullOrWhiteSpace(tier.Name) == false)
                return $"Тир «{tier.Name}»";

            int tierNumber = tierIndex + 1;

            return $"Тир {tierNumber}";
        }
    }
}
