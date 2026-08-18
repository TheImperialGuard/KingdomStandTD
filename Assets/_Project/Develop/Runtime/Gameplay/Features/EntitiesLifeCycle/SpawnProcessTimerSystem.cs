using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class SpawnProcessTimerSystem : IInitializableSystem, IUpdatableSystem
    {
        public ReactiveVariable<float> _initialTime;
        public ReactiveVariable<float> _currentTime;

        public ReactiveVariable<bool> _inSpawnProcess;

        public void OnInit(Entity entity)
        {
            _initialTime = entity.SpawnProcessTimerC.InitialTime;
            _currentTime = entity.SpawnProcessTimerC.CurrentTime;

            _inSpawnProcess = entity.InSpawnProcess;

            _currentTime.Value = 0;

            _inSpawnProcess.Value = true;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inSpawnProcess.Value == false)
                return;

            _currentTime.Value += deltaTime;

            if (TimerIsOver())
                _inSpawnProcess.Value = false;
        }

        private bool TimerIsOver() => _currentTime.Value >= _initialTime.Value;
    }
}
