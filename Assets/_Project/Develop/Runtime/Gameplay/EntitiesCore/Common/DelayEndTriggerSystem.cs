using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common
{
    public abstract class DelayEndTriggerSystem : IInitializableSystem, IDisposableSystem
    {
        protected ReactiveEvent DelayEndEvent;
        protected ReactiveEvent StartProcessEvent;

        protected ReactiveVariable<float> Delay;
        protected ReactiveVariable<float> ProcessCurrentTime;

        protected IDisposable ProcessTimerDisposable;
        protected IDisposable StartProcessEventDisposable;

        private bool _delayEnd;

        public abstract void OnInit(Entity entity);

        public void OnDispose()
        {
            ProcessTimerDisposable.Dispose();
            StartProcessEventDisposable.Dispose();
        }

        protected void OnTimerChanged(float arg1, float currentTime)
        {
            if (_delayEnd)
                return;

            if (currentTime >= Delay.Value)
            {
                DelayEndEvent.Invoke();
                _delayEnd = true;
            }
        }

        protected void OnStartProcess() => _delayEnd = false;
    }
}
