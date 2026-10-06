using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
#endif

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature
{
    public class WavesSpawner : IDisposable
    {
        private readonly EnemiesFactory _enemiesFactory;
        private readonly TimerServiceFactory _timerServiceFactory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly IReadOnlyList<RoadPath> _roadPaths;

        private List<Coroutine> _activeSpawnProcesses = new();
        private List<Entity> _spawnedEnemies = new();

        private int _requestedSpawnProcesses;
        private int _completedSpawnProcesses;

        public WavesSpawner(
            EnemiesFactory enemiesFactory,
            TimerServiceFactory timerServiceFactory,
            ICoroutinesPerformer coroutinesPerformer,
            EntitiesLifeContext entitiesLifeContext,
            IReadOnlyList<RoadPath> roadPaths)
        {
            _enemiesFactory = enemiesFactory;
            _timerServiceFactory = timerServiceFactory;
            _coroutinesPerformer = coroutinesPerformer;
            _entitiesLifeContext = entitiesLifeContext;
            _roadPaths = roadPaths;

            _entitiesLifeContext.Released += OnSomeEntityReleased;
        }

        public IReadOnlyList<Entity> SpawnedEntitites => _spawnedEnemies;

        public bool IsSpawnComplete => _requestedSpawnProcesses > 0 && _completedSpawnProcesses == _requestedSpawnProcesses;

        public void SpawnWave(EnemiesWaveConfig waveConfig)
        {
            Coroutine spawnProcess = _coroutinesPerformer.StartPerform(SpawnProcess(waveConfig));

            _activeSpawnProcesses.Add(spawnProcess);
        }

        public void Stop()
        {
            ClearSpawnProcesses();
        }

        public void Dispose()
        {
            Stop();

            _entitiesLifeContext.Released -= OnSomeEntityReleased;
        }

        private IEnumerator SpawnProcess(EnemiesWaveConfig waveConfig)
        {
            _requestedSpawnProcesses++;

            float startDelay = waveConfig.StartDelay;
            float spawnDelay = waveConfig.DelayBetweenSpawns;

            IReadOnlyList<Waypoint> waypoints = GetRoadPath(waveConfig.PathNumber).Waypoints;
            Vector3 waypointsOffset = PathLaneOffsets.ToLocalOffset(waveConfig.Lane);

            TimerService startTimer;
            TimerService spawnCooldownTimer;

            if (startDelay > 0f)
            {
                startTimer = _timerServiceFactory.Create(startDelay);
                startTimer.Restart();

                yield return new WaitUntil(() => startTimer.IsOver == true);
            }

            spawnCooldownTimer = _timerServiceFactory.Create(spawnDelay);

            for (int repeat = 0; repeat < waveConfig.PatternRepeats; repeat++)
            {
                foreach (WaveEnemyGroup enemyGroup in waveConfig.EnemyGroups)
                {
                    for (int i = 0; i < enemyGroup.Count; i++)
                    {
                        SpawnEnemy(enemyGroup.EnemyConfig, waypoints, waypointsOffset);

                        #if UNITY_EDITOR
                            // SpawnEnemy только что добавил врага в конец списка
                            PlaytestHooks.NotifyEnemySpawned(_spawnedEnemies[_spawnedEnemies.Count - 1], enemyGroup.EnemyConfig, waveConfig);
                        #endif

                        spawnCooldownTimer.Restart();
                        yield return new WaitUntil(() => spawnCooldownTimer.IsOver == true);
                    }
                }
            }

            _completedSpawnProcesses++;
        }

        private RoadPath GetRoadPath(PathNumber number)
        {
            foreach (RoadPath roadPath in _roadPaths)
            {
                if (roadPath.Number == number)
                    return roadPath;
            }

            throw new InvalidOperationException($"На уровне нет дороги с номером {number}");
        }

        private void SpawnEnemy(CharacterConfig enemyConfig, IReadOnlyList<Waypoint> waypoints, Vector3 waypointsOffset)
        {
            Vector3 spawnPos = waypoints.First().transform.position;

            if (waypointsOffset != Vector3.zero)
                spawnPos += waypoints.First().transform.TransformVector(waypointsOffset);

            List<Waypoint> path = GetPathWithoutSpawnPos(waypoints);

            Entity enemy = _enemiesFactory.Create(spawnPos, enemyConfig, path, waypointsOffset);

            _spawnedEnemies.Add(enemy);
        }

        private void ClearSpawnProcesses()
        {
            if (_activeSpawnProcesses.Any())
            {
                foreach (Coroutine process in _activeSpawnProcesses)
                    _coroutinesPerformer.StopPerform(process);
            }
        }

        private void OnSomeEntityReleased(Entity entity)
        {
            if (_spawnedEnemies.Contains(entity))
                _spawnedEnemies.Remove(entity);
        }

        private List<Waypoint> GetPathWithoutSpawnPos(IReadOnlyList<Waypoint> waypoints)
        {
            List<Waypoint> path = new List<Waypoint>();

            for (int i = 0; i < waypoints.Count; i++)
            {
                if (i == 0)
                    continue;

                path.Add(waypoints[i]);
            }

            return path;
        }
    }
}
