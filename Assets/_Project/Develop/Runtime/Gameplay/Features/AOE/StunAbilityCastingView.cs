using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class StunAbilityCastingView : EntityView
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;

        private Entity _entity;

        private ReactiveEvent _startSecondAbilityEvent;

        private IDisposable _startSecondAbilityEventDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _startSecondAbilityEventDisposable?.Dispose();
            _entity.Abilities.Added -= OnAbilityAdded;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;

            _entity.Abilities.Added += OnAbilityAdded;
        }

        private void OnAbilityAdded(Ability ability)
        {
            if (ability is StunAbility == false)
                return;

            _startSecondAbilityEvent = _entity.StartSecondAbilityEvent;
            _startSecondAbilityEventDisposable = _startSecondAbilityEvent.Subscribe(OnAbilityStart);
        }

        private void OnAbilityStart() => CreateEffect();

        private void CreateEffect()
        {
            Instantiate(_effectPrefab, _effectSpawnPoint.position, Quaternion.identity);
        }
    }
}
