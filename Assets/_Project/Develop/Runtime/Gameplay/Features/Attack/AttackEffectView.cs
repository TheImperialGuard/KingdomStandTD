using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackEffectView : EntityView
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;

        private IReadOnlyEvent _attackDelayEndEvent;

        private IDisposable _attackDelayEndEventDisposable;

        public override void Cleanup(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            base.Cleanup(entity);

            _attackDelayEndEventDisposable.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            _attackDelayEndEvent = entity.AttackDelayEndEvent;

            _attackDelayEndEventDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }

        private void OnAttackDelayEnd()
        {
            Instantiate(_effectPrefab, _effectSpawnPoint.position, Quaternion.identity, _effectSpawnPoint);
        }
    }
}
