using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Editor.LevelEditor;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using UnityEngine;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Волны бесконечного режима для инструментов редактора — срез, импорт среза и плейтест из окна редактора уровней. Их
    // собирает тот же генератор, что в игре, по дорогам префаба уровня режима, поэтому это ровно те волны, что выйдут в
    // бою. Волны нумеруются с единицы.
    public static class EndlessEditorWaves
    {
        public const int FirstWaveNumber = 1;

        // Префаб так же, как его грузит игра: Resources.Load по LevelConfig.PrefabPath.
        public static Level LoadLevelPrefab(EndlessModeConfig config)
        {
            if (config.Level == null || string.IsNullOrWhiteSpace(config.Level.PrefabPath))
                return null;

            return Resources.Load<Level>(config.Level.PrefabPath);
        }

        // Генератор — только для конфига без ошибок: на них генерация сломается. Замечания проверки дописываются в issues.
        public static bool TryCreateGenerator(EndlessModeConfig config, List<LevelIssue> issues, out EndlessWavesGenerator generator)
        {
            Level levelPrefab = LoadLevelPrefab(config);
            List<LevelIssue> configIssues = EndlessModeConfigValidator.Validate(config, levelPrefab);

            issues.AddRange(configIssues);

            if (configIssues.Any(issue => issue.IsError))
            {
                generator = null;
                return false;
            }

            generator = new EndlessWavesGenerator(config, levelPrefab.RoadPaths);

            return true;
        }

        public static List<EnemiesWavesStageConfig> Generate(EndlessWavesGenerator generator, int wavesCount)
        {
            List<EnemiesWavesStageConfig> waves = new();

            for (int waveNumber = FirstWaveNumber; waveNumber < FirstWaveNumber + wavesCount; waveNumber++)
                waves.Add(generator.Generate(waveNumber));

            return waves;
        }

        // Статистика волн — как у стейджей кампании, но без последнего стейджа: у бесконечного режима скип есть у каждой
        // волны. Золото на старте и здоровье игрока — из уровня режима.
        public static LevelStats CalculateStats(
            LevelConfig level,
            IReadOnlyList<EnemiesWavesStageConfig> waves,
            LevelPrefabInfo prefab,
            GameConfig gameConfig)
        {
            return LevelStatsCalculator.Calculate(level, waves, hasLastStage: false, prefab, gameConfig);
        }

        // Здоровье врагов каждой посчитанной волны с множителем её номера
        public static List<float> CalculateHealthWithMultiplier(LevelStats stats, EndlessModeConfig config)
        {
            List<float> healths = new();

            for (int i = 0; i < stats.Stages.Count; i++)
                healths.Add(stats.Stages[i].Health * config.HealthMultiplier.GetValue(FirstWaveNumber + i));

            return healths;
        }

        // Золото к началу волны без скипов, будто враги прошлых волн убиты, — как goldAtStartWithoutSkips в срезе.
        public static int CalculateGoldAtStart(LevelConfig level, EndlessWavesGenerator generator, int waveNumber)
        {
            int gold = level.StartGold;

            for (int previousWave = FirstWaveNumber; previousWave < waveNumber; previousWave++)
            {
                foreach (EnemiesWaveConfig stream in generator.Generate(previousWave).EnemiesWaveConfigs)
                    gold += LevelStatsCalculator.CalculateKillGold(stream);
            }

            return gold;
        }
    }
}
