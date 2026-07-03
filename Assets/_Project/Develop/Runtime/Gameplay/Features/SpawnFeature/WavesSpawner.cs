using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature
{
    public class WavesSpawner : IDisposable
    {
        private readonly EnemiesFactory _enemiesFactory;
        private readonly TimerServiceFactory _timerServiceFactory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private List<Coroutine> _activeSpawnProcesses = new();
        private List<Entity> _spawnedEntitites = new();

        private int _requestedSpawnProcesses;
        private int _completedSpawnProcesses;

        private ReactiveVariable<bool> _isSpawnComplete = new();

        public WavesSpawner(
            EnemiesFactory enemiesFactory, 
            TimerServiceFactory timerServiceFactory, 
            ICoroutinesPerformer coroutinesPerformer)
        {
            _enemiesFactory = enemiesFactory;
            _timerServiceFactory = timerServiceFactory;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public IReadOnlyList<Entity> SpawnedEntitites => _spawnedEntitites;

        public IReadOnlyVariable<bool> IsSpawnComplete => _isSpawnComplete;

        public void SpawnWave(EnemiesWaveConfig waveConfig)
        {
            Coroutine spawnProcess = _coroutinesPerformer.StartPerform(SpawnProcess(waveConfig));

            _activeSpawnProcesses.Add(spawnProcess);
        }

        public void Stop()
        {
            ClearSpawnPorecesses();
        }

        public void Dispose()
        {
            Stop();
        }

        private IEnumerator SpawnProcess(EnemiesWaveConfig waveConfig)
        {
            _requestedSpawnProcesses++;

            float startDelay = waveConfig.StartDelay;
            float spawnDelay = waveConfig.DelayBetweenSpawns;

            TimerService startTimer;
            TimerService spawnCooldownTimer;

            if (startDelay > 0f)
            {
                startTimer = _timerServiceFactory.Create(startDelay);
                startTimer.Restart();

                yield return new WaitUntil(() => startTimer.IsOver == true);
            }

            spawnCooldownTimer = _timerServiceFactory.Create(spawnDelay);

            for (int i = 0; i < waveConfig.EnemiesCount; i++)
            {
                SpawnEnemy(waveConfig.EnemyConfig, waveConfig.RoadPath.Waypoints);

                spawnCooldownTimer.Restart();
                yield return new WaitUntil(() => spawnCooldownTimer.IsOver == true);
            }

            _completedSpawnProcesses++;

            CheckSpawnComplete();
        }

        private void SpawnEnemy(CharacterConfig enemyConfig, IReadOnlyList<Waypoint> waypoints)
        {
            Vector3 spawnPos = waypoints.First().transform.position;

            Entity enemy = _enemiesFactory.Create(spawnPos, enemyConfig, waypoints);

            _spawnedEntitites.Add(enemy);
        }

        private void ClearSpawnPorecesses()
        {
            if (_activeSpawnProcesses.Any())
            {
                foreach (Coroutine process in _activeSpawnProcesses)
                    _coroutinesPerformer.StopPerform(process);
            }
        }

        public void CheckSpawnComplete()
        {
            bool result = _requestedSpawnProcesses > 0;

            _isSpawnComplete.Value = result && _completedSpawnProcesses == _requestedSpawnProcesses;
        }
    }
}
