using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class FirstAbilityCooldownByProcessTimeSyncSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _abilityProcessInitialTime;
        private ReactiveVariable<float> _abilityCooldownInitialTime;
        private ReactiveVariable<float> _abilityCooldownModifiedTime;

        private IDisposable _abilityCooldownChangedDisposable;

        public void OnDispose()
        {
            _abilityCooldownChangedDisposable.Dispose();
        }

        public void OnInit(Entity entity)
        {
            _abilityProcessInitialTime = entity.FirstAbilityProcessTimerC.InitialTime;
            _abilityCooldownInitialTime = entity.FirstAbilityCooldownTimerC.InitialTime;
            _abilityCooldownModifiedTime = entity.FirstAbilityCooldownTimerC.ModifiedTime;

            _abilityCooldownChangedDisposable = _abilityCooldownInitialTime.Subscribe(OnAbilityCooldownChanged);
            OnAbilityCooldownChanged(0, _abilityCooldownInitialTime.Value);
        }

        private void OnAbilityCooldownChanged(float arg1, float newCooldown)
        {
            _abilityCooldownModifiedTime.Value = newCooldown - _abilityProcessInitialTime.Value;
        }
    }

    public class SecondAbilityCooldownByProcessTimeSyncSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _abilityProcessInitialTime;
        private ReactiveVariable<float> _abilityCooldownInitialTime;
        private ReactiveVariable<float> _abilityCooldownModifiedTime;

        private IDisposable _abilityCooldownChangedDisposable;

        public void OnDispose()
        {
            _abilityCooldownChangedDisposable.Dispose();
        }

        public void OnInit(Entity entity)
        {
            _abilityProcessInitialTime = entity.SecondAbilityProcessTimerC.InitialTime;
            _abilityCooldownInitialTime = entity.SecondAbilityCooldownTimerC.InitialTime;
            _abilityCooldownModifiedTime = entity.SecondAbilityCooldownTimerC.ModifiedTime;

            _abilityCooldownChangedDisposable = _abilityCooldownInitialTime.Subscribe(OnAbilityCooldownChanged);
            OnAbilityCooldownChanged(0, _abilityCooldownInitialTime.Value);
        }

        private void OnAbilityCooldownChanged(float arg1, float newCooldown)
        {
            _abilityCooldownModifiedTime.Value = newCooldown - _abilityProcessInitialTime.Value;
        }
    }
}
