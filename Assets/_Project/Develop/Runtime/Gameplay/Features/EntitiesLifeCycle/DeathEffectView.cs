using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class DeathEffectView : EntityView
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;

        private IReadOnlyVariable<bool> _isDead;

        private IDisposable _isDeadChangedDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isDeadChangedDisposable.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDead = entity.IsDead;

            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead == true)
                Instantiate(_effectPrefab, _effectSpawnPoint.position, Quaternion.identity);
        }
    }
}
