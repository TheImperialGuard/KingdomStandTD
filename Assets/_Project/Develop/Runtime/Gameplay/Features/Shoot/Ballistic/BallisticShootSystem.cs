using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic
{
    public class BallisticShootSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ProjectilesFactory _projectilesFactory;
        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;
        private readonly StatusesFactory _statusesFactory;

        private ReactiveVariable<TrajectoryResult> _trajectory;

        private ReactiveVariable<bool> _mustStun;
        private ReactiveVariable<bool> _mustCreateFragments;

        private ReactiveEvent _attackDelayEndEvent;

        private Entity _shooterEntity;
        private Transform _shootPoint;

        private IDisposable _attackDelayDisposable;

        public BallisticShootSystem(
            ProjectilesFactory projectilesFactory, 
            AreaEntitiesDetectorService areaEntitiesDetectorService, 
            StatusesFactory statusesFactory)
        {
            _projectilesFactory = projectilesFactory;
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
            _statusesFactory = statusesFactory;
        }

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.AttackDelayEndEvent;

            _trajectory = entity.BallisticTrajectory;

            _mustStun = entity.MustStunOnAttack;
            _mustCreateFragments = entity.MustCreateFragmentsOnProjectileDeath;

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

            if (_mustStun.Value == true)
            {
                projectile.AddStunDuration(_shooterEntity.StunDuration);
                projectile.AddSystem(new StunEnemiesInAreaOnDeathSystem(_areaEntitiesDetectorService, _statusesFactory));
            }

            if (_mustCreateFragments.Value == true)
            {
                projectile.AddFragmentsDamage(_shooterEntity.FragmentsDamage);
                projectile.AddFragmentsCount(_shooterEntity.FragmentsCount);
                projectile.AddSystem(new CreateFragmentsOnDeathSystem(_projectilesFactory));
            }

            projectile.Rigidbody.AddForce(_trajectory.Value.launchVelocity, ForceMode.VelocityChange);
        }
    }
}
