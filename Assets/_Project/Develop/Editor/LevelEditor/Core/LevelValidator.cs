using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Проверка уровня. Ошибки — то, на чём игра сломается; предупреждения — то, что работает, но скорее всего не задумано.
    public static class LevelValidator
    {
        public static List<LevelIssue> Validate(LevelConfig config, LevelPrefabInfo prefab)
        {
            List<LevelIssue> issues = new();

            ValidateEconomy(config, issues);
            ValidatePrefab(config, prefab, issues);
            ValidateStages(config, prefab, issues);

            return issues;
        }

        private static void ValidateEconomy(LevelConfig config, List<LevelIssue> issues)
        {
            if (config.StartGold < LevelConfigLimits.MIN_START_GOLD)
                issues.Add(LevelIssue.Error($"Стартовое золото отрицательное: {config.StartGold}"));

            if (config.StartPlayerHealth < LevelConfigLimits.MIN_PLAYER_HEALTH)
            {
                issues.Add(LevelIssue.Error(
                    $"Здоровье игрока {config.StartPlayerHealth}, нужно не меньше {LevelConfigLimits.MIN_PLAYER_HEALTH}"));
            }
        }

        private static void ValidatePrefab(LevelConfig config, LevelPrefabInfo prefab, List<LevelIssue> issues)
        {
            switch (prefab.Status)
            {
                case LevelPrefabStatus.PathMissing:
                    issues.Add(LevelIssue.Error("Не задан путь к префабу уровня (PrefabPath)"));
                    return;

                case LevelPrefabStatus.PrefabMissing:
                    issues.Add(LevelIssue.Error($"Префаб уровня не найден в Resources по пути «{config.PrefabPath}»"));
                    return;

                case LevelPrefabStatus.LevelComponentMissing:
                    issues.Add(LevelIssue.Error($"На префабе {prefab.AssetPath} нет компонента Level"));
                    return;

                case LevelPrefabStatus.Found:
                    break;
            }

            // TowersPlaceholdersService обходит места под башни без проверки: пустой элемент сломает запуск уровня.
            if (prefab.MissingTowerSlotsCount > 0)
                issues.Add(LevelIssue.Error($"Пустых элементов в списке мест под башни: {prefab.MissingTowerSlotsCount}"));

            if (prefab.TowerSlotsCount == 0)
                issues.Add(LevelIssue.Warning("На префабе нет ни одного места под башню"));

            if (prefab.MissingRoadsCount > 0)
                issues.Add(LevelIssue.Error($"Пустых элементов в списке дорог префаба: {prefab.MissingRoadsCount}"));

            foreach (LevelRoad road in prefab.Roads)
                ValidateRoad(road, issues);

            foreach (IGrouping<PathNumber, LevelRoad> sameNumberRoads in prefab.Roads.GroupBy(road => road.Number))
            {
                if (sameNumberRoads.Count() > 1)
                {
                    string roadNames = string.Join(", ", sameNumberRoads.Select(road => road.Name));
                    issues.Add(LevelIssue.Error($"Номер дороги {sameNumberRoads.Key} повторяется: {roadNames}"));
                }
            }
        }

        private static void ValidateRoad(LevelRoad road, List<LevelIssue> issues)
        {
            if (road.MissingWaypointsCount > 0)
                issues.Add(LevelIssue.Error($"Дорога {road.Number} ({road.Name}): пустых вейпоинтов {road.MissingWaypointsCount}"));

            if (road.WaypointsCount < LevelPrefabReader.MIN_WAYPOINTS_COUNT)
            {
                issues.Add(LevelIssue.Error(
                    $"Дорога {road.Number} ({road.Name}): вейпоинтов {road.WaypointsCount}, нужно не меньше {LevelPrefabReader.MIN_WAYPOINTS_COUNT}"));
            }
        }

        private static void ValidateStages(LevelConfig config, LevelPrefabInfo prefab, List<LevelIssue> issues)
        {
            IReadOnlyList<EnemiesWavesStageConfig> stages = config.Stages;

            if (stages.Count == 0)
            {
                issues.Add(LevelIssue.Error("В уровне нет стейджей"));
                return;
            }

            for (int i = 0; i < stages.Count; i++)
            {
                bool isLastStage = i == stages.Count - 1;
                ValidateStage(stages[i], i, isLastStage, prefab, issues);
            }
        }

        // В последнем стейдже скип не предлагается, а конец его времени не останавливает спавн:
        // игра ждёт, пока заспавнятся все волны. Поэтому предупреждения о времени к нему не относятся.
        private static void ValidateStage(
            EnemiesWavesStageConfig stage,
            int stageIndex,
            bool isLastStage,
            LevelPrefabInfo prefab,
            List<LevelIssue> issues)
        {
            if (stage.StageTime < LevelConfigLimits.MIN_STAGE_TIME)
            {
                string stageTimeText = LevelSliceNumbers.ToText(stage.StageTime);
                issues.Add(LevelIssue.Error($"Отрицательное время стейджа: {stageTimeText} с", stageIndex));
            }

            if (stage.TimeToSkipStage < LevelConfigLimits.MIN_TIME_TO_SKIP_STAGE)
            {
                string skipTimeText = LevelSliceNumbers.ToText(stage.TimeToSkipStage);
                issues.Add(LevelIssue.Error($"Отрицательное время до скипа: {skipTimeText} с", stageIndex));
            }

            if (isLastStage == false && LevelSliceNumbers.Round(stage.TimeToSkipStage) >= LevelSliceNumbers.Round(stage.StageTime))
            {
                string skipTimeText = LevelSliceNumbers.ToText(stage.TimeToSkipStage);
                string stageTimeText = LevelSliceNumbers.ToText(stage.StageTime);
                issues.Add(LevelIssue.Warning(
                    $"Время до скипа ({skipTimeText} с) не меньше времени стейджа ({stageTimeText} с) — скип не появится",
                    stageIndex));
            }

            IReadOnlyList<EnemiesWaveConfig> waves = LevelStatsCalculator.GetWaves(stage);

            if (waves.Count == 0)
            {
                issues.Add(LevelIssue.Error("В стейдже нет волн", stageIndex));
                return;
            }

            for (int i = 0; i < waves.Count; i++)
                ValidateWave(waves[i], stage, stageIndex, i, isLastStage, prefab, issues);
        }

        private static void ValidateWave(
            EnemiesWaveConfig wave,
            EnemiesWavesStageConfig stage,
            int stageIndex,
            int waveIndex,
            bool isLastStage,
            LevelPrefabInfo prefab,
            List<LevelIssue> issues)
        {
            IReadOnlyList<WaveEnemyGroup> groups = LevelStatsCalculator.GetGroups(wave);

            if (groups.Count == 0)
                issues.Add(LevelIssue.Error("Пустой шаблон волны: нет ни одной группы врагов", stageIndex, waveIndex));

            for (int i = 0; i < groups.Count; i++)
                ValidateGroup(groups[i], stageIndex, waveIndex, i, issues);

            if (wave.PatternRepeats < LevelConfigLimits.MIN_PATTERN_REPEATS)
            {
                issues.Add(LevelIssue.Error(
                    $"Повторов шаблона {wave.PatternRepeats}, нужно не меньше {LevelConfigLimits.MIN_PATTERN_REPEATS}",
                    stageIndex,
                    waveIndex));
            }

            if (wave.StartDelay < LevelConfigLimits.MIN_START_DELAY)
            {
                string startDelayText = LevelSliceNumbers.ToText(wave.StartDelay);
                issues.Add(LevelIssue.Error($"Отрицательная задержка старта: {startDelayText} с", stageIndex, waveIndex));
            }

            if (LevelSliceNumbers.Round(wave.DelayBetweenSpawns) < LevelSliceNumbers.Round(LevelConfigLimits.MIN_DELAY_BETWEEN_SPAWNS))
            {
                string delayText = LevelSliceNumbers.ToText(wave.DelayBetweenSpawns);
                string minDelayText = LevelSliceNumbers.ToText(LevelConfigLimits.MIN_DELAY_BETWEEN_SPAWNS);
                issues.Add(LevelIssue.Error($"Интервал спавна {delayText} с, нужно не меньше {minDelayText} с", stageIndex, waveIndex));
            }

            if (prefab.Status == LevelPrefabStatus.Found && prefab.Roads.Any(road => road.Number == wave.PathNumber) == false)
                issues.Add(LevelIssue.Error($"На префабе нет дороги {wave.PathNumber}", stageIndex, waveIndex));

            float lastSpawnTime = LevelStatsCalculator.CalculateLastSpawnTime(wave);

            if (isLastStage == false && LevelSliceNumbers.Round(lastSpawnTime) > LevelSliceNumbers.Round(stage.StageTime))
            {
                string lastSpawnText = LevelSliceNumbers.ToText(lastSpawnTime);
                string stageTimeText = LevelSliceNumbers.ToText(stage.StageTime);
                issues.Add(LevelIssue.Warning(
                    $"Последний спавн на {lastSpawnText} с — позже конца стейджа ({stageTimeText} с)",
                    stageIndex,
                    waveIndex));
            }
        }

        private static void ValidateGroup(WaveEnemyGroup group, int stageIndex, int waveIndex, int groupIndex, List<LevelIssue> issues)
        {
            if (group.EnemyConfig == null)
            {
                issues.Add(LevelIssue.Error("Враг не выбран или его ассет удалён", stageIndex, waveIndex, groupIndex));
            }
            else if ((group.EnemyConfig is MeleeConfig) == false)
            {
                string enemyName = group.EnemyConfig.name;
                string configType = group.EnemyConfig.GetType().Name;
                issues.Add(LevelIssue.Error(
                    $"Враг {enemyName} ({configType}) не поддерживается: EnemiesFactory создаёт только MeleeConfig",
                    stageIndex,
                    waveIndex,
                    groupIndex));
            }

            if (group.Count < LevelConfigLimits.MIN_GROUP_COUNT)
            {
                issues.Add(LevelIssue.Error(
                    $"Врагов в группе {group.Count}, нужно не меньше {LevelConfigLimits.MIN_GROUP_COUNT}",
                    stageIndex,
                    waveIndex,
                    groupIndex));
            }
        }
    }
}
