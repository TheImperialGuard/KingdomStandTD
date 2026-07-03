using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    public interface IGameMode : IDisposable
    {
        IReadOnlyEvent<LevelResults> End { get; }

        void Start();
        void Update(float deltaTime);
    }
}