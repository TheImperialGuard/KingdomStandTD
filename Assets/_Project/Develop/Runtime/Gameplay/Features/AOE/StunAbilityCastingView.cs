using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.SimpleMono;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    // Каст оглушения на кристаллах башни: за время задержки кристаллы раскручиваются и разгораются,
    // в момент срабатывания из каждого бьёт ледяной луч в цель, и кристаллы замедляются до обычной скорости.
    // Отменённая способность гасит подготовку и тоже замедляет кристаллы
    public class StunAbilityCastingView : ParticleEffectsEntityView
    {
        private const float NormalSpinMultiplier = 1f;

        [SerializeField] private ParticleSystem _crystalChargeEffectPrefab;
        [SerializeField] private ParticleSystem _beamEffectPrefab;
        [SerializeField] private ParticleSystem _beamsImpactEffectPrefab;
        [SerializeField] private Transform[] _crystals;
        [SerializeField] private MonoRotator _crystalsRotator;
        [SerializeField, Min(1)] private float _chargedSpinMultiplier = 10f;
        [SerializeField, Min(0)] private float _spinDownDuration = 1f;
        [SerializeField, Min(0)] private float _beamFlightDuration = 0.07f;

        private readonly List<ParticleSystem> _chargeEffects = new();

        private Entity _entity;

        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<float> _delay;

        private ReactiveEvent _startSecondAbilityEvent;
        private ReactiveEvent _secondAbilityDelayEndEvent;
        private ReactiveEvent _secondAbilityCanceledEvent;

        private IDisposable _startSecondAbilityEventDisposable;
        private IDisposable _secondAbilityDelayEndEventDisposable;
        private IDisposable _secondAbilityCanceledEventDisposable;

        private Tween _spinTween;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _startSecondAbilityEventDisposable?.Dispose();
            _secondAbilityDelayEndEventDisposable?.Dispose();
            _secondAbilityCanceledEventDisposable?.Dispose();
            _entity.Abilities.Added -= OnAbilityAdded;

            _spinTween?.Kill();
            _crystalsRotator.SpeedMultiplier = NormalSpinMultiplier;

            _chargeEffects.Clear();
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

            _currentTarget = _entity.CurrentTarget;
            _delay = _entity.SecondAbilityDelayModifiedTime;

            _startSecondAbilityEvent = _entity.StartSecondAbilityEvent;
            _secondAbilityDelayEndEvent = _entity.SecondAbilityDelayEndEvent;
            _secondAbilityCanceledEvent = _entity.SecondAbilityCanceledEvent;

            _startSecondAbilityEventDisposable = _startSecondAbilityEvent.Subscribe(OnAbilityStart);
            _secondAbilityDelayEndEventDisposable = _secondAbilityDelayEndEvent.Subscribe(OnAbilityDelayEnd);
            _secondAbilityCanceledEventDisposable = _secondAbilityCanceledEvent.Subscribe(OnAbilityCanceled);
        }

        // Эффект подготовки привязан к кристаллу и крутится вместе с ним
        private void OnAbilityStart()
        {
            foreach (Transform crystal in _crystals)
                _chargeEffects.Add(EffectsPool.Spawn(_crystalChargeEffectPrefab, crystal.position, crystal));

            SpinCrystals(_chargedSpinMultiplier, _delay.Value, Ease.InOutSine);
        }

        private void OnAbilityDelayEnd()
        {
            FinishCharge();

            Entity target = _currentTarget.Value;

            // Цель пропала, и замены не нашлось — оглушения нет, лучей тоже (StunEntitiesInAreaSecondAbilitySystem)
            if (target == null || target.IsInit == false)
                return;

            Vector3 aimPoint = GetAimPoint(target);

            foreach (Transform crystal in _crystals)
                LaunchBeam(crystal.position, aimPoint);

            DOVirtual.DelayedCall(_beamFlightDuration, () => EffectsPool.Spawn(_beamsImpactEffectPrefab, aimPoint), false);
        }

        private void OnAbilityCanceled() => FinishCharge();

        // Подготовка гаснет: новые частицы не появляются, выпущенные догорают
        private void FinishCharge()
        {
            foreach (ParticleSystem chargeEffect in _chargeEffects)
                chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            _chargeEffects.Clear();

            SpinCrystals(NormalSpinMultiplier, _spinDownDuration, Ease.OutSine);
        }

        private void LaunchBeam(Vector3 from, Vector3 to)
        {
            ParticleSystem beam = EffectsPool.Spawn(_beamEffectPrefab, from);

            beam.transform
                .DOMove(to, _beamFlightDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => beam.Stop(true, ParticleSystemStopBehavior.StopEmitting));
        }

        private void SpinCrystals(float speedMultiplier, float duration, Ease ease)
        {
            _spinTween?.Kill();

            _spinTween = DOTween
                .To(() => _crystalsRotator.SpeedMultiplier, value => _crystalsRotator.SpeedMultiplier = value, speedMultiplier, duration)
                .SetEase(ease);
        }

        private Vector3 GetAimPoint(Entity target)
        {
            if (target.TryGetBodyCollider(out CapsuleCollider body))
                return body.bounds.center;

            return target.Transform.position;
        }
    }
}
