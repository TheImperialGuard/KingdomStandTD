using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Рост здоровья врагов бесконечного режима. Враг из волны сразу после спавна получает множитель здоровья своей волны:
    // номер волны берётся по потоку, из которого он вышел, поэтому хвосты прошлых волн остаются со своим множителем.
    // Призванные враги создаются мимо спавнера и остаются с базовым здоровьем.
    public class EndlessHealthMultiplierService : IDisposable
    {
        private readonly WavesSpawner _wavesSpawner;
        private readonly EndlessStagesSource _stagesSource;
        private readonly EndlessModeConfig _config;

        public EndlessHealthMultiplierService(
            WavesSpawner wavesSpawner,
            EndlessStagesSource stagesSource,
            EndlessModeConfig config)
        {
            _wavesSpawner = wavesSpawner;
            _stagesSource = stagesSource;
            _config = config;

            _wavesSpawner.EnemySpawned += OnEnemySpawned;
        }

        public void Dispose()
        {
            _wavesSpawner.EnemySpawned -= OnEnemySpawned;
        }

        private void OnEnemySpawned(Entity enemy, EnemiesWaveConfig stream)
        {
            if (_stagesSource.TryGetWaveNumber(stream, out int waveNumber) == false)
                return;

            ReactiveVariable<float> maxHealth = enemy.MaxHealth;

            maxHealth.Value *= _config.HealthMultiplier.GetValue(waveNumber);
            enemy.CurrentHealth.Value = maxHealth.Value;
        }
    }
}
