using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common
{
    public abstract class CooldownTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        protected ReactiveVariable<float> CurrentTime;
        protected ReactiveVariable<float> InitialTime;
        protected ReactiveVariable<bool> InCooldown;

        protected ReactiveEvent StartCooldownEvent;

        protected IDisposable StartCooldownEventDisposable;

        public abstract void OnInit(Entity entity);

        public void OnUpdate(float deltaTime)
        {
            if (InCooldown.Value == false)
                return;

            CurrentTime.Value -= deltaTime;

            if (CooldownIsOver())
            {
                InCooldown.Value = false;
            }
        }

        public void OnDispose()
        {
            StartCooldownEventDisposable.Dispose();
        }

        protected void OnStartCooldownEvent()
        {
            CurrentTime.Value = InitialTime.Value;

            InCooldown.Value = true;
        }

        private bool CooldownIsOver() => CurrentTime.Value <= 0;
    }
}
