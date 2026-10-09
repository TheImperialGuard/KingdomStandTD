using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EnemyAbilities
{
    // Запускает анимацию способности и растягивает её на длительность каста через Speed Multiplier её состояния.
    [RequireComponent(typeof(Animator))]
    public class AbilityCastAnimationView : EntityView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _castClip;

        [SerializeField] private string _castTriggerKeyName;
        [SerializeField] private string _castSpeedKeyName;

        private ReactiveVariable<float> _castDuration;

        private IDisposable _startAbilityEventDisposable;

        private int CastTriggerKey => Animator.StringToHash(_castTriggerKeyName);

        private int CastSpeedKey => Animator.StringToHash(_castSpeedKeyName);

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _startAbilityEventDisposable?.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetStartFirstAbilityEvent(out ReactiveEvent startAbilityEvent) == false)
                return;

            _castDuration = entity.FirstAbilityProcessTimerC.ModifiedTime;

            _startAbilityEventDisposable = startAbilityEvent.Subscribe(OnAbilityStarted);
        }

        private void OnValidate()
        {
            _animator = _animator != null ? _animator : GetComponent<Animator>();
        }

        private void OnAbilityStarted()
        {
            _animator.SetFloat(CastSpeedKey, _castClip.length / _castDuration.Value);
            _animator.SetTrigger(CastTriggerKey);
        }
    }
}
