using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic
{
    public class BallisticShootSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ProjectilesFactory _projectilesFactory;

        private ReactiveVariable<TrajectoryResult> _trajectory;

        private ReactiveEvent _attackDelayEndEvent;

        private Entity _shooterEntity;
        private Transform _shootPoint;

        private IDisposable _attackDelayDisposable;

        public BallisticShootSystem(ProjectilesFactory projectilesFactory)
        {
            _projectilesFactory = projectilesFactory;
        }

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.AttackDelayEndEvent;

            _trajectory = entity.BallisticTrajectory;

            _shooterEntity = entity;
            _shootPoint = entity.ShootPoint;

            _attackDelayDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }

        public void OnDispose()
        {
            _attackDelayDisposable.Dispose();
        }

        private void OnAttackDelayEnd()
        {
            Shoot();
        }

        private void Shoot()
        {
            Entity projectile = _projectilesFactory.Create(
                _shooterEntity.ProjectileType.Value, 
                _shootPoint.position, 
                _trajectory.Value.launchDirection, 
                _shooterEntity);

            projectile.Rigidbody.AddForce(_trajectory.Value.launchVelocity, ForceMode.VelocityChange);
        }
    }
}
