using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public class StagesFactory
    {
        private readonly DIContainer _container;

        public StagesFactory(DIContainer container)
        {
            _container = container;
        }

        public IStage Create(StageConfig config)
        {
            switch (config)
            {
                case EnemiesWavesStageConfig enemiesWaveStageConfig:
                    return new EnemiesWavesStage(
                        enemiesWaveStageConfig,
                        _container.Resolve<TimerServiceFactory>(),
                        _container.Resolve<WavesSpawner>());

                default:
                    throw new ArgumentException($"Not supported {config.GetType()} type config");
            }
        }
    }
}
