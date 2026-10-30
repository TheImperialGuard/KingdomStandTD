using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;
using Random = System.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Собирает волну бесконечного режима по её номеру — обычный EnemiesWavesStageConfig, который играет конвейер стейджей.
    // Состояния забега у генератора нет: случайность берётся из зерна конфига и номера волны, и волна N всегда одна и та
    // же. Поэтому его же зовёт таблица волн в инспекторе конфига режима.
    //
    // Число врагов волны делится между тирами по их долям методом наибольшего остатка; доля тира, у которого на этой
    // волне ещё нет открытых врагов, достаётся остальным пропорционально. Внутри тира — несколько случайных видов из
    // открытых. Каждая группа делится между всеми дорогами уровня; на дороге сначала идут слабые, потом сильнее — в
    // порядке тиров. Волна длится, пока выходят враги самой длинной дороги, плюс передышка; досрочный вызов следующей
    // открывается, когда вышел последний враг.
    public class EndlessWavesGenerator
    {
        // Зерно волны — зерно конфига × множитель + номер волны: у разных зёрен волны не совпадают, пока номер меньше него
        private const int SeedMultiplier = 100003;

        private const float MinDelayBetweenSpawns = 0.1f;
        private const float MinRestTime = 0f;
        private const float StartDelay = 0f;

        // Остаток, который уже дал врага, и вес, которому враги не положены, в дележе по наибольшему остатку
        private const double UsedRemainder = -1d;
        private const double NoWeightRemainder = double.NegativeInfinity;

        private readonly EndlessModeConfig _config;
        private readonly List<PathNumber> _pathNumbers = new();

        public EndlessWavesGenerator(EndlessModeConfig config, IReadOnlyList<RoadPath> roadPaths)
        {
            _config = config;

            foreach (RoadPath roadPath in roadPaths)
            {
                if (_pathNumbers.Contains(roadPath.Number) == false)
                    _pathNumbers.Add(roadPath.Number);
            }

            if (_pathNumbers.Count == 0)
                throw new ArgumentException("Для волн бесконечного режима на уровне нужна хотя бы одна дорога");
        }

        // Номера всех дорог уровня: волна идёт по каждой
        public IReadOnlyList<PathNumber> PathNumbers => _pathNumbers;

        public EnemiesWavesStageConfig Generate(int waveNumber)
        {
            Random random = new Random(unchecked(_config.Seed * SeedMultiplier + waveNumber));

            int enemiesCount = Mathf.Max(0, Mathf.RoundToInt(_config.EnemiesCount.GetValue(waveNumber)));
            float delayBetweenSpawns = Mathf.Max(MinDelayBetweenSpawns, _config.DelayBetweenSpawns.GetValue(waveNumber));
            float restTime = Mathf.Max(MinRestTime, _config.RestTime.GetValue(waveNumber));

            List<WaveEnemyGroup> groups = ComposeGroups(waveNumber, enemiesCount, random);
            List<List<WaveEnemyGroup>> pathsGroups = SplitByPaths(groups);

            List<EnemiesWaveConfig> waves = new();
            int longestPathEnemiesCount = 0;

            for (int i = 0; i < _pathNumbers.Count; i++)
            {
                List<WaveEnemyGroup> pathGroups = pathsGroups[i];

                if (pathGroups.Count == 0)
                    continue;

                waves.Add(new EnemiesWaveConfig(pathGroups, _pathNumbers[i], PathLane.Center, StartDelay, delayBetweenSpawns));

                longestPathEnemiesCount = Mathf.Max(longestPathEnemiesCount, CountEnemies(pathGroups));
            }

            float spawnDuration = GetSpawnDuration(longestPathEnemiesCount, delayBetweenSpawns);

            return new EnemiesWavesStageConfig(waves, spawnDuration + restTime, spawnDuration);
        }

        // Группы волны в порядке тиров, по группе на каждый выбранный вид
        private List<WaveEnemyGroup> ComposeGroups(int waveNumber, int enemiesCount, Random random)
        {
            IReadOnlyList<EndlessTier> tiers = _config.Tiers;

            List<List<MeleeConfig>> tiersOpenEnemies = new();
            double[] shares = new double[tiers.Count];

            for (int i = 0; i < tiers.Count; i++)
            {
                List<MeleeConfig> openEnemies = GetOpenEnemies(tiers[i], waveNumber);

                tiersOpenEnemies.Add(openEnemies);

                // Тир без открытых врагов выпадает из дележа, и его доля сама расходится по остальным пропорционально
                if (openEnemies.Count > 0)
                    shares[i] = Mathf.Max(0f, tiers[i].Share.GetValue(waveNumber));
            }

            int[] tiersEnemiesCounts = DistributeByLargestRemainder(enemiesCount, shares);

            List<WaveEnemyGroup> groups = new();

            for (int i = 0; i < tiers.Count; i++)
                AddTierGroups(groups, tiersOpenEnemies[i], tiers[i].KindsPerWave, tiersEnemiesCounts[i], random);

            return groups;
        }

        private static List<MeleeConfig> GetOpenEnemies(EndlessTier tier, int waveNumber)
        {
            List<MeleeConfig> openEnemies = new();

            foreach (EndlessTierEnemy tierEnemy in tier.Enemies)
            {
                if (tierEnemy.EnemyConfig != null && tierEnemy.FirstWave <= waveNumber)
                    openEnemies.Add(tierEnemy.EnemyConfig);
            }

            return openEnemies;
        }

        // Каждому — целая часть его квоты, остаток раздаётся по одному тем, у кого дробная часть больше
        // (при равенстве — тому, кто раньше в списке). Сумма сходится с total.
        private static int[] DistributeByLargestRemainder(int total, double[] weights)
        {
            int[] counts = new int[weights.Length];
            double weightsSum = 0d;

            foreach (double weight in weights)
                weightsSum += weight;

            if (total <= 0 || weightsSum <= 0d)
                return counts;

            double[] remainders = new double[weights.Length];
            int distributedCount = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                double quota = total * weights[i] / weightsSum;

                counts[i] = (int)Math.Floor(quota);
                remainders[i] = weights[i] > 0d ? quota - counts[i] : NoWeightRemainder;
                distributedCount += counts[i];
            }

            for (int leftCount = total - distributedCount; leftCount > 0; leftCount--)
            {
                int index = GetMaxIndex(remainders);

                counts[index]++;
                remainders[index] = UsedRemainder;
            }

            return counts;
        }

        private static int GetMaxIndex(double[] values)
        {
            int maxIndex = 0;

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > values[maxIndex])
                    maxIndex = i;
            }

            return maxIndex;
        }

        // Виды тира на волну — случайные из открытых, сколько разрешает тир. Враги тира делятся между ними поровну,
        // остаток — первым по жребию. Группы идут в порядке врагов в тире.
        private static void AddTierGroups(
            List<WaveEnemyGroup> groups,
            List<MeleeConfig> openEnemies,
            int kindsPerWave,
            int tierEnemiesCount,
            Random random)
        {
            if (tierEnemiesCount <= 0 || openEnemies.Count == 0)
                return;

            int kindsCount = Mathf.Min(kindsPerWave, openEnemies.Count, tierEnemiesCount);
            List<int> pickedIndices = PickRandomIndices(openEnemies.Count, kindsCount, random);

            int[] kindsEnemiesCounts = new int[openEnemies.Count];
            int baseCount = tierEnemiesCount / kindsCount;
            int extraCount = tierEnemiesCount % kindsCount;

            for (int i = 0; i < pickedIndices.Count; i++)
                kindsEnemiesCounts[pickedIndices[i]] = i < extraCount ? baseCount + 1 : baseCount;

            for (int i = 0; i < openEnemies.Count; i++)
            {
                if (kindsEnemiesCounts[i] > 0)
                    groups.Add(new WaveEnemyGroup(openEnemies[i], kindsEnemiesCounts[i]));
            }
        }

        // Частичное перемешивание Фишера — Йетса: первые pickCount индексов в случайном порядке
        private static List<int> PickRandomIndices(int count, int pickCount, Random random)
        {
            List<int> indices = new(count);

            for (int i = 0; i < count; i++)
                indices.Add(i);

            for (int i = 0; i < pickCount; i++)
            {
                int swapIndex = random.Next(i, count);

                (indices[i], indices[swapIndex]) = (indices[swapIndex], indices[i]);
            }

            indices.RemoveRange(pickCount, count - pickCount);

            return indices;
        }

        // Каждая группа делится между дорогами поровну. Остаток раздаётся по очереди, и очередь идёт сквозь все группы
        // волны: лишние враги разных групп не скапливаются на первой дороге.
        private List<List<WaveEnemyGroup>> SplitByPaths(List<WaveEnemyGroup> groups)
        {
            int pathsCount = _pathNumbers.Count;
            List<List<WaveEnemyGroup>> pathsGroups = new();

            for (int i = 0; i < pathsCount; i++)
                pathsGroups.Add(new List<WaveEnemyGroup>());

            int nextPathIndex = 0;

            foreach (WaveEnemyGroup group in groups)
            {
                int[] pathsEnemiesCounts = new int[pathsCount];
                int baseCount = group.Count / pathsCount;
                int extraCount = group.Count % pathsCount;

                for (int i = 0; i < pathsCount; i++)
                    pathsEnemiesCounts[i] = baseCount;

                for (int i = 0; i < extraCount; i++)
                {
                    pathsEnemiesCounts[nextPathIndex]++;
                    nextPathIndex = (nextPathIndex + 1) % pathsCount;
                }

                for (int i = 0; i < pathsCount; i++)
                {
                    if (pathsEnemiesCounts[i] > 0)
                        pathsGroups[i].Add(new WaveEnemyGroup(group.EnemyConfig, pathsEnemiesCounts[i]));
                }
            }

            return pathsGroups;
        }

        private static int CountEnemies(List<WaveEnemyGroup> groups)
        {
            int count = 0;

            foreach (WaveEnemyGroup group in groups)
                count += group.Count;

            return count;
        }

        // Первый враг дороги выходит сразу, каждый следующий — через интервал
        private static float GetSpawnDuration(int pathEnemiesCount, float delayBetweenSpawns)
        {
            if (pathEnemiesCount == 0)
                return 0f;

            return (pathEnemiesCount - 1) * delayBetweenSpawns;
        }
    }
}
