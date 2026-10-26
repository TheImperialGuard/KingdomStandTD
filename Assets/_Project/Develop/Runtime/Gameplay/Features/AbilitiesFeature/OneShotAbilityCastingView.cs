using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    // Подготовка ваншота: эффект играет от начала способности до выстрела, его длина подогнана под Delay конфига.
    // Отменённая способность гасит эффект: новые частицы не появляются, выпущенные догорают
    public class OneShotAbilityCastingView : ParticleEffectsEntityView
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;

        private Entity _entity;

        private ReactiveEvent _startFirstAbilityEvent;
        private ReactiveEvent _firstAbilityDelayEndEvent;
        private ReactiveEvent _firstAbilityCanceledEvent;

        private IDisposable _startFirstAbilityEventDisposable;
        private IDisposable _firstAbilityDelayEndEventDisposable;
        private IDisposable _firstAbilityCanceledEventDisposable;

        private ParticleSystem _effect;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _startFirstAbilityEventDisposable?.Dispose();
            _firstAbilityDelayEndEventDisposable?.Dispose();
            _firstAbilityCanceledEventDisposable?.Dispose();
            _entity.Abilities.Added -= OnAbilityAdded;

            _effect = null;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;

            _entity.Abilities.Added += OnAbilityAdded;
        }

        private void OnAbilityAdded(Ability ability)
        {
            if (ability is OneShotAttackAbility == false)
                return;

            _startFirstAbilityEvent = _entity.StartFirstAbilityEvent;
            _firstAbilityDelayEndEvent = _entity.FirstAbilityDelayEndEvent;
            _firstAbilityCanceledEvent = _entity.FirstAbilityCanceledEvent;

            _startFirstAbilityEventDisposable = _startFirstAbilityEvent.Subscribe(OnAbilityStart);
            _firstAbilityDelayEndEventDisposable = _firstAbilityDelayEndEvent.Subscribe(OnAbilityDelayEnd);
            _firstAbilityCanceledEventDisposable = _firstAbilityCanceledEvent.Subscribe(OnAbilityCanceled);
        }

        private void OnAbilityStart() => _effect = EffectsPool.Spawn(_effectPrefab, _effectSpawnPoint.position);

        // Ссылка обнуляется, чтобы не погасить экземпляр, который пул уже отдал другой башне
        private void OnAbilityDelayEnd() => _effect = null;

        private void OnAbilityCanceled()
        {
            if (_effect == null)
                return;

            _effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            _effect = null;
        }
    }
}
