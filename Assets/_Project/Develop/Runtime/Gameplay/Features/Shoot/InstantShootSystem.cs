using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class InstantShootSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ProjectilesFactory _projectilesFactory;

        private ReactiveVariable<InstantShotDirectionArgs> _directionArgs;

        private ReactiveEvent _attackDelayEndEvent;

        private Entity _shooterEntity;
        private Transform _shootPoint;

        private IDisposable _attackDelayDisposable;

        public InstantShootSystem(ProjectilesFactory projectilesFactory)
        {
            _projectilesFactory = projectilesFactory;
        }

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.AttackDelayEndEvent;

            _directionArgs = entity.InstantShotDirection;

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
            Shoot(_directionArgs.Value.Direction, _directionArgs.Value.ProjectileCounts);
        }

        private void Shoot(Vector3 direction, int projectileCounts)
        {
            Vector2 perpindicular = Vector2.Perpendicular(new Vector2(direction.x, direction.z)).normalized;

            float offesetBetweenProjectiles = 0.6f;

            for (int i = 0; i < projectileCounts; i++)
            {
                Vector2 offset = perpindicular * (-offesetBetweenProjectiles / 2f * (projectileCounts - 1) + i * offesetBetweenProjectiles);
                Vector3 position = new Vector3(_shootPoint.position.x + offset.x, _shootPoint.position.y, _shootPoint.position.z + offset.y);

                _projectilesFactory.Create(_shooterEntity.ProjectileType.Value, position, direction, _shooterEntity);
            }
        }
    }
}
