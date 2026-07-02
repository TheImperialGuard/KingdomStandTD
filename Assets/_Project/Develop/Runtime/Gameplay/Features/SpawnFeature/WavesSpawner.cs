using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
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

        public WavesSpawner(
            EnemiesFactory enemiesFactory, 
            TimerServiceFactory timerServiceFactory, 
            ICoroutinesPerformer coroutinesPerformer)
        {
            _enemiesFactory = enemiesFactory;
            _timerServiceFactory = timerServiceFactory;
            _coroutinesPerformer = coroutinesPerformer;
        }

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
        }

        private void SpawnEnemy(CharacterConfig enemyConfig, IReadOnlyList<Waypoint> waypoints)
        {
            Vector3 spawnPos = waypoints.First().transform.position;

            _enemiesFactory.Create(spawnPos, enemyConfig, waypoints);
        }

        private void ClearSpawnPorecesses()
        {
            if (_activeSpawnProcesses.Any())
            {
                foreach (Coroutine process in _activeSpawnProcesses)
                    _coroutinesPerformer.StopPerform(process);
            }
        }
    }
}
