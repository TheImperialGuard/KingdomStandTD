using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class FirstAbilityInstantShootSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ProjectilesFactory _projectilesFactory;

        private ReactiveVariable<InstantShotDirectionArgs> _directionArgs;
        private ReactiveVariable<Entity> _currentTarget;

        private ReactiveEvent _firstAbilityDelayEndEvent;

        private Entity _shooterEntity;
        private Transform _shootPoint;

        private IDisposable _abilityDelayDisposable;

        public FirstAbilityInstantShootSystem(ProjectilesFactory projectilesFactory)
        {
            _projectilesFactory = projectilesFactory;
        }

        public void OnInit(Entity entity)
        {
            _firstAbilityDelayEndEvent = entity.FirstAbilityDelayEndEvent;

            _directionArgs = entity.InstantShotDirection;
            _currentTarget = entity.CurrentTarget;

            _shooterEntity = entity;
            _shootPoint = entity.ShootPoint;

            _abilityDelayDisposable = _firstAbilityDelayEndEvent.Subscribe(OnAbilityDelayEnd);
        }

        public void OnDispose()
        {
            _abilityDelayDisposable.Dispose();
        }

        private void OnAbilityDelayEnd()
        {
            Vector3 shootDirection = _directionArgs.Value.Direction;

            if (_currentTarget.Value.Transform != null)
                shootDirection = (_currentTarget.Value.Transform.position - _shootPoint.position).normalized;

            Shoot(shootDirection, _directionArgs.Value.ProjectileCounts);
        }

        private void Shoot(Vector3 direction, int projectileCounts)
        {
            Vector2 perpindicular = Vector2.Perpendicular(new Vector2(direction.x, direction.z)).normalized;

            float offesetBetweenProjectiles = 0.6f;

            for (int i = 0; i < projectileCounts; i++)
            {
                Vector2 offset = perpindicular * (-offesetBetweenProjectiles / 2f * (projectileCounts - 1) + i * offesetBetweenProjectiles);
                Vector3 position = new Vector3(_shootPoint.position.x + offset.x, _shootPoint.position.y, _shootPoint.position.z + offset.y);

                _projectilesFactory.Create(_shooterEntity.FirstAbilityProjectileType.Value, position, direction, _shooterEntity);
            }
        }
    }
}
