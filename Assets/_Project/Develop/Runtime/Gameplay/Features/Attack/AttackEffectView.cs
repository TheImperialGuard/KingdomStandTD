using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackEffectView : ParticleEffectsEntityView
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        [SerializeField] private bool _alignWithSpawnPoint;

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
            ParticleSystem effect = EffectsPool.Spawn(_effectPrefab, _effectSpawnPoint.position, _effectSpawnPoint);

            // Пул ставит эффект без поворота; направленный эффект, например язык огня из ствола, разворачиваем по точке
            if (_alignWithSpawnPoint)
                effect.transform.rotation = _effectSpawnPoint.rotation;
        }
    }
}
