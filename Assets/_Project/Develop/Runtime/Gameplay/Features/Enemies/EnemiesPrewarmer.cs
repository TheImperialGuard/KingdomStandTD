using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Кладёт в пул объекты врагов уровня до начала боя: по одному на вид, а призываемым — весь строй, который призыв
    // создаёт за один кадр. Обычные враги выходят по одному, поэтому дальше пул дорастёт в бою сам
    public class EnemiesPrewarmer
    {
        private const int InstancesPerKind = 1;

        private readonly LevelConfig _levelConfig;
        private readonly MonoEntitiesFactory _monoEntitiesFactory;

        public EnemiesPrewarmer(LevelConfig levelConfig, MonoEntitiesFactory monoEntitiesFactory)
        {
            _levelConfig = levelConfig;
            _monoEntitiesFactory = monoEntitiesFactory;
        }

        public void Prewarm()
        {
            Dictionary<string, int> pathToCount = new();

            foreach (EnemiesWavesStageConfig stage in _levelConfig.Stages)
            {
                foreach (EnemiesWaveConfig wave in stage.EnemiesWaveConfigs)
                {
                    foreach (WaveEnemyGroup group in wave.EnemyGroups)
                        AddCount(group.EnemyConfig, InstancesPerKind, pathToCount);
                }
            }

            foreach (KeyValuePair<string, int> prefabCount in pathToCount)
                _monoEntitiesFactory.Prewarm(prefabCount.Key, prefabCount.Value);
        }

        // Вид может прийти и в волне, и призывом — берётся наибольшее число
        private void AddCount(CharacterConfig config, int count, Dictionary<string, int> pathToCount)
        {
            if (pathToCount.TryGetValue(config.PrefabPath, out int currentCount) && currentCount >= count)
                return;

            pathToCount[config.PrefabPath] = count;

            if (config is MeleeConfig meleeConfig && meleeConfig.Ability is SummonAbilityConfig summonConfig)
                AddCount(summonConfig.SummonedConfig, summonConfig.Count, pathToCount);
        }
    }
}
