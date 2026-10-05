using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Статистика уровня по LevelConfig и дорогам префаба. Формулы повторяют рантайм: порядок спавна — WavesSpawner,
    // сопротивление урону — EntitiesHelper, золото за скип — EarnGoldOnSkipStageService.
    // Время в пути и пик врагов на поле — оценки без убийств: каждый враг доходит до базы.
    public static class LevelStatsCalculator
    {
        private static readonly IReadOnlyList<EnemiesWaveConfig> _noWaves = Array.Empty<EnemiesWaveConfig>();
        private static readonly IReadOnlyList<WaveEnemyGroup> _noGroups = Array.Empty<WaveEnemyGroup>();

        public static LevelStats Calculate(LevelConfig config, LevelPrefabInfo prefab, GameConfig gameConfig)
        {
            LevelStats levelStats = new();
            List<List<EnemySpawn>> stagesSpawns = new();
            IReadOnlyList<EnemiesWavesStageConfig> stages = config.Stages;

            for (int i = 0; i < stages.Count; i++)
            {
                bool isLastStage = i == stages.Count - 1;
                List<EnemySpawn> stageSpawns = new();

                levelStats.Stages.Add(CalculateStage(stages[i], isLastStage, config, prefab, gameConfig, stageSpawns));
                stagesSpawns.Add(stageSpawns);
            }

            SumStages(config, levelStats);
            CalculateDurations(stages, levelStats);
            CalculatePeaks(stages, stagesSpawns, levelStats);

            levelStats.Roads = CalculateRoads(config, prefab);

            return levelStats;
        }

        // Как EntitiesHelper.CalculateFinalDamage: сопротивление режет урон только своего типа.
        public static float CalculateEffectiveHealth(MeleeConfig enemy, DamageTypes damageType)
        {
            if (enemy.DamageResistanceType == DamageTypes.None || enemy.DamageResistanceType != damageType)
                return enemy.MaxHealth;

            float damageFactor = 1f - enemy.DamageResistanceIndex;

            if (damageFactor <= 0f)
                return float.PositiveInfinity;

            return enemy.MaxHealth / damageFactor;
        }

        // Как EarnGoldOnSkipStageService: золото за оставшееся время, если скипнуть, как только разрешено.
        // В последнем стейдже скип не предлагается.
        public static int CalculateMaxSkipGold(EnemiesWavesStageConfig stage, bool isLastStage, GameConfig gameConfig)
        {
            if (isLastStage || gameConfig == null)
                return 0;

            float remainingTime = stage.StageTime - stage.TimeToSkipStage;

            if (remainingTime <= 0f)
                return 0;

            float earnedGold = remainingTime * gameConfig.GoldIndexForRemainingStageSeconds;
            int gold = (int)Math.Round(earnedGold, MidpointRounding.AwayFromZero);

            return Math.Min(gold, gameConfig.MaxGoldForSkipStage);
        }

        public static int CountEnemies(EnemiesWaveConfig wave)
        {
            int patternCount = GetGroups(wave).Sum(group => Math.Max(0, group.Count));

            return Math.Max(0, wave.PatternRepeats) * patternCount;
        }

        // Как WavesSpawner: первый враг выходит через StartDelay, каждый следующий — через DelayBetweenSpawns.
        public static float CalculateLastSpawnTime(EnemiesWaveConfig wave)
        {
            int enemiesCount = CountEnemies(wave);

            if (enemiesCount == 0)
                return wave.StartDelay;

            return wave.StartDelay + (enemiesCount - 1) * wave.DelayBetweenSpawns;
        }

        // Порядок спавна как в WavesSpawner: повторы шаблона × группы × количество в группе;
        // первый враг выходит через StartDelay, каждый следующий — через DelayBetweenSpawns.
        public static IEnumerable<WaveSpawn> EnumerateSpawns(EnemiesWaveConfig wave)
        {
            int repeats = Math.Max(0, wave.PatternRepeats);
            int spawnIndex = 0;

            for (int repeat = 0; repeat < repeats; repeat++)
            {
                foreach (WaveEnemyGroup group in GetGroups(wave))
                {
                    int count = Math.Max(0, group.Count);

                    for (int i = 0; i < count; i++)
                    {
                        float spawnTime = wave.StartDelay + spawnIndex * wave.DelayBetweenSpawns;

                        yield return new WaveSpawn(spawnTime, group.EnemyConfig);
                        spawnIndex++;
                    }
                }
            }
        }

        public static IReadOnlyList<EnemiesWaveConfig> GetWaves(EnemiesWavesStageConfig stage)
            => stage.EnemiesWaveConfigs ?? _noWaves;

        public static IReadOnlyList<WaveEnemyGroup> GetGroups(EnemiesWaveConfig wave)
            => wave.EnemyGroups ?? _noGroups;

        private static StageStats CalculateStage(
            EnemiesWavesStageConfig stage,
            bool isLastStage,
            LevelConfig config,
            LevelPrefabInfo prefab,
            GameConfig gameConfig,
            List<EnemySpawn> stageSpawns)
        {
            StageStats stageStats = new();
            List<RoadTrafficStats> wavesTraffic = new();

            foreach (EnemiesWaveConfig wave in GetWaves(stage))
            {
                List<EnemySpawn> waveSpawns = CollectSpawns(wave, FindRoad(prefab, wave.PathNumber));
                WaveStats waveStats = CalculateWave(wave, waveSpawns);

                stageStats.Waves.Add(waveStats);
                stageSpawns.AddRange(waveSpawns);

                stageStats.EnemiesCount += waveStats.EnemiesCount;
                CountEnemiesByType(wave, stageStats.EnemiesByType);
                stageStats.Health += waveStats.Health;
                stageStats.EffectiveHealthPhysic += SumStat(wave, enemy => CalculateEffectiveHealth(enemy, DamageTypes.Physic));
                stageStats.EffectiveHealthMagic += SumStat(wave, enemy => CalculateEffectiveHealth(enemy, DamageTypes.Magic));
                stageStats.KillGold += waveStats.KillGold;
                stageStats.BreakthroughDamage += waveStats.BreakthroughDamage;
                stageStats.SpawnEnd = Math.Max(stageStats.SpawnEnd, waveStats.SpawnEnd);

                wavesTraffic.Add(new RoadTrafficStats
                {
                    PathNumber = wave.PathNumber,
                    Lane = wave.Lane,
                    WavesCount = 1,
                    EnemiesCount = waveStats.EnemiesCount,
                    Health = waveStats.Health,
                });
            }

            stageStats.WavesCount = stageStats.Waves.Count;
            stageStats.HealthPerSecond = stage.StageTime > 0f ? stageStats.Health / stage.StageTime : 0f;
            stageStats.MaxSkipGold = CalculateMaxSkipGold(stage, isLastStage, gameConfig);
            stageStats.BreakthroughThreat = CalculateThreat(stageStats.BreakthroughDamage, config.StartPlayerHealth);
            stageStats.SpawnOverflow = Math.Max(0f, stageStats.SpawnEnd - stage.StageTime);
            stageStats.LastArrival = CalculateLastArrival(stageSpawns);
            stageStats.Traffic = MergeTraffic(wavesTraffic);

            return stageStats;
        }

        private static WaveStats CalculateWave(EnemiesWaveConfig wave, IReadOnlyList<EnemySpawn> spawns)
        {
            return new WaveStats
            {
                EnemiesCount = spawns.Count,
                Health = SumStat(wave, enemy => enemy.MaxHealth),
                KillGold = SumStat(wave, enemy => enemy.GoldOnDeath),
                BreakthroughDamage = SumStat(wave, enemy => enemy.DamageOnFinishPath),
                SpawnEnd = CalculateLastSpawnTime(wave),
                FirstArrival = CalculateFirstArrival(spawns),
                LastArrival = CalculateLastArrival(spawns),
            };
        }

        private static List<EnemySpawn> CollectSpawns(EnemiesWaveConfig wave, LevelRoad road)
        {
            float? length = GetLength(road, wave.Lane);

            return EnumerateSpawns(wave)
                .Select(spawn => new EnemySpawn(spawn.Time, CalculateTravelTime(spawn.Enemy, length)))
                .ToList();
        }

        private static float? CalculateTravelTime(CharacterConfig enemyConfig, float? length)
        {
            if (TryGetMelee(enemyConfig, out MeleeConfig enemy) == false || length.HasValue == false || enemy.MoveSpeed <= 0f)
                return null;

            return length.Value / enemy.MoveSpeed;
        }

        private static float? CalculateFirstArrival(IReadOnlyList<EnemySpawn> spawns)
        {
            if (spawns.Count == 0 || spawns.Any(spawn => spawn.ArrivalTime.HasValue == false))
                return null;

            return spawns.Min(spawn => spawn.ArrivalTime.Value);
        }

        private static float? CalculateLastArrival(IReadOnlyList<EnemySpawn> spawns)
        {
            if (spawns.Count == 0 || spawns.Any(spawn => spawn.ArrivalTime.HasValue == false))
                return null;

            return spawns.Max(spawn => spawn.ArrivalTime.Value);
        }

        // Сумма по всем врагам волны: шаблон групп × число повторов. Враги не MeleeConfig не учитываются:
        // игра их создать не может, об этом сообщает валидатор.
        private static float SumStat(EnemiesWaveConfig wave, Func<MeleeConfig, float> selector)
        {
            float patternSum = 0f;

            foreach (WaveEnemyGroup group in GetGroups(wave))
            {
                if (TryGetMelee(group.EnemyConfig, out MeleeConfig enemy))
                    patternSum += Math.Max(0, group.Count) * selector(enemy);
            }

            return Math.Max(0, wave.PatternRepeats) * patternSum;
        }

        private static int SumStat(EnemiesWaveConfig wave, Func<MeleeConfig, int> selector)
        {
            int patternSum = 0;

            foreach (WaveEnemyGroup group in GetGroups(wave))
            {
                if (TryGetMelee(group.EnemyConfig, out MeleeConfig enemy))
                    patternSum += Math.Max(0, group.Count) * selector(enemy);
            }

            return Math.Max(0, wave.PatternRepeats) * patternSum;
        }

        private static void CountEnemiesByType(EnemiesWaveConfig wave, SortedDictionary<string, int> enemiesByType)
        {
            int repeats = Math.Max(0, wave.PatternRepeats);

            foreach (WaveEnemyGroup group in GetGroups(wave))
            {
                int count = repeats * Math.Max(0, group.Count);

                if (group.EnemyConfig == null || count == 0)
                    continue;

                AddCount(enemiesByType, group.EnemyConfig.name, count);
            }
        }

        private static void AddCount(SortedDictionary<string, int> counts, string key, int count)
        {
            counts.TryGetValue(key, out int currentCount);
            counts[key] = currentCount + count;
        }

        private static bool TryGetMelee(CharacterConfig enemyConfig, out MeleeConfig enemy)
        {
            enemy = enemyConfig as MeleeConfig;

            return enemy != null;
        }

        // Как WavesSpawner: берётся первая дорога с нужным номером.
        private static LevelRoad FindRoad(LevelPrefabInfo prefab, PathNumber number)
            => prefab.Roads.FirstOrDefault(road => road.Number == number);

        private static float? GetLength(LevelRoad road, PathLane lane)
        {
            if (road == null || road.HasValidPath == false || road.Lengths.TryGetValue(lane, out float length) == false)
                return null;

            return length;
        }

        private static float CalculateThreat(int breakthroughDamage, int playerHealth)
            => playerHealth > 0 ? (float)breakthroughDamage / playerHealth : 0f;

        private static List<RoadTrafficStats> MergeTraffic(IEnumerable<RoadTrafficStats> entries)
        {
            return entries
                .GroupBy(entry => (entry.PathNumber, entry.Lane))
                .OrderBy(group => group.Key.PathNumber)
                .ThenBy(group => group.Key.Lane)
                .Select(group => new RoadTrafficStats
                {
                    PathNumber = group.Key.PathNumber,
                    Lane = group.Key.Lane,
                    WavesCount = group.Sum(entry => entry.WavesCount),
                    EnemiesCount = group.Sum(entry => entry.EnemiesCount),
                    Health = group.Sum(entry => entry.Health),
                })
                .ToList();
        }

        private static void SumStages(LevelConfig config, LevelStats levelStats)
        {
            int goldWithoutSkips = config.StartGold;
            int goldWithMaxSkips = config.StartGold;

            foreach (StageStats stageStats in levelStats.Stages)
            {
                stageStats.GoldAtStartWithoutSkips = goldWithoutSkips;
                stageStats.GoldAtStartWithMaxSkips = goldWithMaxSkips;

                goldWithoutSkips += stageStats.KillGold;
                goldWithMaxSkips += stageStats.KillGold + stageStats.MaxSkipGold;

                levelStats.WavesCount += stageStats.WavesCount;
                levelStats.EnemiesCount += stageStats.EnemiesCount;
                levelStats.Health += stageStats.Health;
                levelStats.EffectiveHealthPhysic += stageStats.EffectiveHealthPhysic;
                levelStats.EffectiveHealthMagic += stageStats.EffectiveHealthMagic;
                levelStats.KillGold += stageStats.KillGold;
                levelStats.MaxSkipGold += stageStats.MaxSkipGold;
                levelStats.BreakthroughDamage += stageStats.BreakthroughDamage;

                foreach (KeyValuePair<string, int> enemyCount in stageStats.EnemiesByType)
                    AddCount(levelStats.EnemiesByType, enemyCount.Key, enemyCount.Value);
            }

            levelStats.StagesCount = levelStats.Stages.Count;
            levelStats.TotalGoldWithoutSkips = goldWithoutSkips;
            levelStats.TotalGoldWithMaxSkips = goldWithMaxSkips;
            levelStats.BreakthroughThreat = CalculateThreat(levelStats.BreakthroughDamage, config.StartPlayerHealth);
            levelStats.Traffic = MergeTraffic(levelStats.Stages.SelectMany(stageStats => stageStats.Traffic));
        }

        // Длительность — момент, когда WavesSpawner закончит спавн всех волн: раньше победа невозможна.
        // Без скипов стейдж длится StageTime, с максимумом скипов — до первой возможности скипа.
        private static void CalculateDurations(IReadOnlyList<EnemiesWavesStageConfig> stages, LevelStats levelStats)
        {
            float stageStartWithoutSkips = 0f;
            float stageStartWithMaxSkips = 0f;

            foreach (EnemiesWavesStageConfig stage in stages)
            {
                float spawnCompletion = CalculateSpawnCompletion(stage);

                levelStats.DurationWithoutSkips = Math.Max(levelStats.DurationWithoutSkips, stageStartWithoutSkips + spawnCompletion);
                levelStats.DurationWithMaxSkips = Math.Max(levelStats.DurationWithMaxSkips, stageStartWithMaxSkips + spawnCompletion);

                stageStartWithoutSkips += stage.StageTime;
                stageStartWithMaxSkips += Math.Min(stage.TimeToSkipStage, stage.StageTime);
            }
        }

        // Спавн волны завершается через DelayBetweenSpawns после последнего врага.
        private static float CalculateSpawnCompletion(EnemiesWavesStageConfig stage)
        {
            float spawnCompletion = 0f;

            foreach (EnemiesWaveConfig wave in GetWaves(stage))
            {
                float waveCompletion = wave.StartDelay + CountEnemies(wave) * wave.DelayBetweenSpawns;
                spawnCompletion = Math.Max(spawnCompletion, waveCompletion);
            }

            return spawnCompletion;
        }

        // Пик врагов на поле без убийств: враг на поле от спавна до прихода на базу, стейджи идут без скипов.
        // Если время в пути хоть одного врага неизвестно, пик не считается.
        private static void CalculatePeaks(
            IReadOnlyList<EnemiesWavesStageConfig> stages,
            IReadOnlyList<List<EnemySpawn>> stagesSpawns,
            LevelStats levelStats)
        {
            List<EnemySpawn> levelSpawns = new();
            List<float> stageStarts = new();
            float stageStart = 0f;

            for (int i = 0; i < stages.Count; i++)
            {
                stageStarts.Add(stageStart);

                foreach (EnemySpawn spawn in stagesSpawns[i])
                {
                    if (spawn.TravelTime.HasValue == false)
                        return;

                    levelSpawns.Add(spawn.ShiftedBy(stageStart));
                }

                stageStart += stages[i].StageTime;
            }

            for (int i = 0; i < stages.Count; i++)
            {
                bool isLastStage = i == stages.Count - 1;
                float windowStart = stageStarts[i];
                float windowEnd = isLastStage ? float.PositiveInfinity : stageStarts[i + 1];
                int stagePeak = CountOnField(levelSpawns, windowStart);

                foreach (EnemySpawn spawn in levelSpawns)
                {
                    if (spawn.SpawnTime > windowStart && spawn.SpawnTime < windowEnd)
                        stagePeak = Math.Max(stagePeak, CountOnField(levelSpawns, spawn.SpawnTime));
                }

                levelStats.Stages[i].PeakEnemies = stagePeak;
                levelStats.PeakEnemies = Math.Max(levelStats.PeakEnemies ?? 0, stagePeak);
            }
        }

        private static int CountOnField(IReadOnlyList<EnemySpawn> spawns, float time)
            => spawns.Count(spawn => spawn.SpawnTime <= time && spawn.ArrivalTime > time);

        private static List<RoadStats> CalculateRoads(LevelConfig config, LevelPrefabInfo prefab)
        {
            List<RoadStats> roadsStats = new();

            foreach (LevelRoad road in prefab.Roads)
            {
                RoadStats roadStats = new()
                {
                    PathNumber = road.Number,
                    Name = road.Name,
                    WaypointsCount = road.WaypointsCount,
                    LengthCenter = GetLength(road, PathLane.Center),
                    LengthLeft = GetLength(road, PathLane.Left),
                    LengthRight = GetLength(road, PathLane.Right),
                };

                if (roadStats.LengthCenter.HasValue)
                    AddTimesToBase(config, roadStats);

                roadsStats.Add(roadStats);
            }

            return roadsStats;
        }

        private static void AddTimesToBase(LevelConfig config, RoadStats roadStats)
        {
            foreach (EnemiesWavesStageConfig stage in config.Stages)
            {
                foreach (EnemiesWaveConfig wave in GetWaves(stage))
                {
                    if (wave.PathNumber != roadStats.PathNumber)
                        continue;

                    foreach (WaveEnemyGroup group in GetGroups(wave))
                    {
                        float? travelTime = CalculateTravelTime(group.EnemyConfig, roadStats.LengthCenter);

                        if (travelTime.HasValue)
                            roadStats.TimeToBase[group.EnemyConfig.name] = travelTime.Value;
                    }
                }
            }
        }

        private readonly struct EnemySpawn
        {
            public EnemySpawn(float spawnTime, float? travelTime)
            {
                SpawnTime = spawnTime;
                TravelTime = travelTime;
            }

            public float SpawnTime { get; }
            public float? TravelTime { get; }
            public float? ArrivalTime => SpawnTime + TravelTime;

            public EnemySpawn ShiftedBy(float offset) => new(SpawnTime + offset, TravelTime);
        }
    }
}
