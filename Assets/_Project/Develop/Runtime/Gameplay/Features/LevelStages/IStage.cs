using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public interface IStage : IDisposable
    {
        IReadOnlyEvent Completed { get; }
        IReadOnlyEvent CanBeSkiped { get; }

        float RemainingTime { get; }

        void Start();
        void Update(float deltaTime);
        void Cleanup();
    }
}
