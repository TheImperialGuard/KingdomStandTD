using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public interface IStage : IDisposable
    {
        IReadOnlyEvent Completed { get; }

        void Start();
        void Update(float deltaTime);
        void Cleanup();
    }

    public class StagesFactory
    {
        private readonly DIContainer _container;

        public StagesFactory(DIContainer container)
        {
            _container = container;
        }

    }
}
