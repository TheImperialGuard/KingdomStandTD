using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Строка таблицы волн в инспекторе режима: что генератор собрал для одной волны. Виды врагов — по именам ассетов
    // (у волков нет CharacterName), в порядке тиров.
    public class EndlessWaveSummary
    {
        private const int NotFoundIndex = -1;
        private const string KindsSeparator = ", ";

        private EndlessWaveSummary(
            int waveNumber,
            int enemiesCount,
            IReadOnlyList<int> tiersEnemiesCounts,
            string composition,
            float spawnDuration,
            float waveDuration)
        {
            WaveNumber = waveNumber;
            EnemiesCount = enemiesCount;
            TiersEnemiesCounts = tiersEnemiesCounts;
            Composition = composition;
            SpawnDuration = spawnDuration;
            WaveDuration = waveDuration;
        }

        public int WaveNumber { get; }
        public int EnemiesCount { get; }
        public IReadOnlyList<int> TiersEnemiesCounts { get; }
        public string Composition { get; }
        public float SpawnDuration { get; }
        public float WaveDuration { get; }

        public static EndlessWaveSummary Create(int waveNumber, EnemiesWavesStageConfig wave, EndlessModeConfig config)
        {
            Dictionary<CharacterConfig, int> kindsCounts = new();

            foreach (EnemiesWaveConfig stream in wave.EnemiesWaveConfigs)
            {
                foreach (WaveEnemyGroup group in stream.EnemyGroups)
                {
                    kindsCounts.TryGetValue(group.EnemyConfig, out int count);
                    kindsCounts[group.EnemyConfig] = count + group.Count;
                }
            }

            int[] tiersEnemiesCounts = new int[config.Tiers.Count];
            int enemiesCount = 0;

            foreach (KeyValuePair<CharacterConfig, int> kindCount in kindsCounts)
            {
                int tierIndex = GetTierIndex(kindCount.Key, config);

                if (tierIndex != NotFoundIndex)
                    tiersEnemiesCounts[tierIndex] += kindCount.Value;

                enemiesCount += kindCount.Value;
            }

            IEnumerable<string> kinds = kindsCounts
                .OrderBy(kindCount => GetTierIndex(kindCount.Key, config))
                .ThenBy(kindCount => GetIndexInTier(kindCount.Key, config))
                .Select(kindCount => $"{kindCount.Key.name} ×{kindCount.Value}");

            string composition = string.Join(KindsSeparator, kinds);

            return new EndlessWaveSummary(
                waveNumber,
                enemiesCount,
                tiersEnemiesCounts,
                composition,
                wave.TimeToSkipStage,
                wave.StageTime);
        }

        private static int GetTierIndex(CharacterConfig enemyConfig, EndlessModeConfig config)
        {
            for (int i = 0; i < config.Tiers.Count; i++)
            {
                if (IndexOf(config.Tiers[i], enemyConfig) != NotFoundIndex)
                    return i;
            }

            return NotFoundIndex;
        }

        private static int GetIndexInTier(CharacterConfig enemyConfig, EndlessModeConfig config)
        {
            int tierIndex = GetTierIndex(enemyConfig, config);

            if (tierIndex == NotFoundIndex)
                return NotFoundIndex;

            return IndexOf(config.Tiers[tierIndex], enemyConfig);
        }

        private static int IndexOf(EndlessTier tier, CharacterConfig enemyConfig)
        {
            for (int i = 0; i < tier.Enemies.Count; i++)
            {
                if (tier.Enemies[i].EnemyConfig == enemyConfig)
                    return i;
            }

            return NotFoundIndex;
        }
    }
}
