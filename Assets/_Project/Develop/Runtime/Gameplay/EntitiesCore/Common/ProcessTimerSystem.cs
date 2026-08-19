using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common
{
    public abstract class ProcessTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        protected ReactiveVariable<float> CurrentTime;

        protected ReactiveVariable<bool> InProcess;

        protected ReactiveEvent StartProcessEvent;

        protected IDisposable StartProcessEventDisposable;

        public abstract void OnInit(Entity entity);

        public void OnDispose()
        {
            StartProcessEventDisposable.Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            if (InProcess.Value == false)
                return;

            CurrentTime.Value += deltaTime;
        }

        protected void OnStartProcessEvent()
        {
            CurrentTime.Value = 0;
        }
    }
}
