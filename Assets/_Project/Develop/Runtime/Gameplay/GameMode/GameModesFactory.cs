using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    public partial class GameModesFactory
    {
        private readonly DIContainer _container;

        public GameModesFactory(DIContainer container)
        {
            _container = container;
        }

        public IGameMode Create(GameModes mode)
        {
            switch (mode)
            {
                case GameModes.Basic:
                    return new BasicGameMode(
                        _container.Resolve<StagesCycle>(),
                        _container.Resolve<WavesSpawner>(),
                        _container.Resolve<PlayerHealth>(),
                        _container.Resolve<GameplayPopupService>());

                default:
                    throw new ArgumentException($"Not supported {nameof(mode)} game mode");
            }
        }
    }
}