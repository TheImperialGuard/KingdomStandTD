using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles
{
    public class ProjectileTrailView : ParticleEffectsEntityView
    {
        [SerializeField] private ParticleSystem _trailPrefab;
        [SerializeField] private Transform _projectile;

        private IReadOnlyVariable<bool> _isDead;

        private IDisposable _isDeadChangedDisposable;

        private ParticleSystem _trail;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isDeadChangedDisposable?.Dispose();

            // Снаряд освобождён без смерти — след гаснет так же, как при смерти
            FinishTrail();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDead = entity.IsDead;

            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);

            _trail = EffectsPool.Spawn(_trailPrefab, _projectile.position);
        }

        private void Update()
        {
            if (_trail != null)
            {
                _trail.transform.position = _projectile.position;
            }
        }

        private void OnIsDeadChanged(bool arg1, bool isDead) => FinishTrail();

        // След перестаёт выпускать частицы во всех слоях и возвращается в пул, когда погаснут уже выпущенные.
        // Ссылка обнуляется, чтобы вью не двигала экземпляр, который пул уже отдал другому снаряду
        private void FinishTrail()
        {
            if (_trail == null)
                return;

            _trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            _trail = null;
        }
    }
}
