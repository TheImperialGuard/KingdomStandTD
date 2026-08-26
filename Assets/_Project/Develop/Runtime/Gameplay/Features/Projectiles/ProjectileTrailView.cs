using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles
{
    public class ProjectileTrailView : EntityView
    {
        [SerializeField] private ParticleSystem _trailPrefab;
        [SerializeField] private Transform _projectile;

        private IReadOnlyVariable<bool> _isDead;

        private IDisposable _isDeadChangedDisposable;

        private ParticleSystem _trail;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDead = entity.IsDead;

            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);

            _trail = Instantiate(_trailPrefab, _projectile.position, Quaternion.identity, null);
        }

        private void Update()
        {
            if (_trail != null)
            {
                _trail.transform.position = _projectile.position;
            }
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            var main = _trail.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
    }
}
