using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class OneShotAbilitySoundEffectView : EntityView
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField, Range(-3f, 3f)] private float _pitchUpperLimit = 1f;
        [SerializeField, Range(-3f, 3f)] private float _pitchLowerLimit = 1f;

        private Entity _entity;

        private IReadOnlyEvent _abilityDelayEndEvent;

        private IDisposable _abilityDelayEndEventDisposable;

        private void OnValidate()
        {
            if (_pitchLowerLimit > _pitchUpperLimit)
                _pitchUpperLimit = _pitchLowerLimit;
        }

        public override void Cleanup(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            base.Cleanup(entity);

            _abilityDelayEndEventDisposable?.Dispose();
            _entity.Abilities.Added -= OnAbilityAdded;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            _entity = entity;

            _entity.Abilities.Added += OnAbilityAdded;
        }

        private void OnAbilityAdded(Ability ability)
        {
            if (ability is OneShotAttackAbility == false)
                return;

            _abilityDelayEndEvent = _entity.FirstAbilityDelayEndEvent;

            _abilityDelayEndEventDisposable = _abilityDelayEndEvent.Subscribe(OnAbilityDelayEnd);
        }

        private void OnAbilityDelayEnd()
        {
            _audioSource.pitch = Random.Range(_pitchLowerLimit, _pitchUpperLimit);
            _audioSource.PlayOneShot(_audioClip);
        }
    }
}
