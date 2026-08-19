using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common
{
    public abstract class EndProcessSystem : IInitializableSystem, IDisposableSystem
    {
        protected ReactiveEvent EndProcessEvent;

        protected ReactiveVariable<bool> InProcess;

        protected ReactiveVariable<float> ProcessInitialTime;
        protected ReactiveVariable<float> ProcessCurrentTime;

        protected IDisposable CurrentTimeDisposable;

        public abstract void OnInit(Entity entity);

        public void OnDispose()
        {
            CurrentTimeDisposable.Dispose();
        }

        protected void OnCurrentTimeChanged(float arg1, float currentTime)
        {
            if (TimerIsDone(currentTime))
            {
                InProcess.Value = false;
                EndProcessEvent.Invoke();
            }
        }

        private bool TimerIsDone(float currentTime) => currentTime >= ProcessInitialTime.Value;
    }
}
