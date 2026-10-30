using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    // Что понадобится стейджам уровня ещё до старта: виды врагов — для прогрева пула, дороги — для превью маршрутов.
    // Кампания собирает это по своим стейджам, бесконечный режим — по тирам врагов и всем дорогам уровня.
    public class LevelStagesContent
    {
        public LevelStagesContent(IReadOnlyList<CharacterConfig> enemies, IReadOnlyList<PathNumber> pathNumbers)
        {
            Enemies = enemies;
            PathNumbers = pathNumbers;
        }

        public IReadOnlyList<CharacterConfig> Enemies { get; }
        public IReadOnlyList<PathNumber> PathNumbers { get; }

        public static LevelStagesContent CollectFrom(IEnumerable<EnemiesWavesStageConfig> stages)
        {
            List<CharacterConfig> enemies = new();
            List<PathNumber> pathNumbers = new();

            foreach (EnemiesWavesStageConfig stage in stages)
            {
                foreach (EnemiesWaveConfig wave in stage.EnemiesWaveConfigs)
                {
                    AddDistinct(pathNumbers, wave.PathNumber);

                    foreach (WaveEnemyGroup group in wave.EnemyGroups)
                        AddDistinct(enemies, group.EnemyConfig);
                }
            }

            return new LevelStagesContent(enemies, pathNumbers);
        }

        private static void AddDistinct<T>(List<T> items, T item)
        {
            if (items.Contains(item) == false)
                items.Add(item);
        }
    }
}
