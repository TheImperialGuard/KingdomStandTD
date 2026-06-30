using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class InstantShootSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly EntitiesFactory _entitiesFactory;

        private ReactiveVariable<InstantShotDirectionArgs> _directionArgs;
        private ReactiveVariable<float> _damage;

        private ReactiveEvent _attackRequest;
        private ReactiveEvent _endAttackEvent;

        private Entity _shooterEntity;
        private Transform _shootPoint;

        private IDisposable _attackRequestDisposable;

        public InstantShootSystem(EntitiesFactory entitiesFactory)
        {
            _entitiesFactory = entitiesFactory;
        }

        public void OnInit(Entity entity)
        {
            _attackRequest = entity.StartAttackRequest;
            _endAttackEvent = entity.EndAttackEvent;

            _directionArgs = entity.InstantShotDirection;
            _damage = entity.InstantAttackDamage;

            _shooterEntity = entity;
            _shootPoint = entity.ShootPoint;

            _attackRequestDisposable = _attackRequest.Subscribe(OnAttackRequest);
        }

        public void OnDispose()
        {
            _attackRequestDisposable.Dispose();
        }

        private void OnAttackRequest()
        {
            Shoot(_directionArgs.Value.Direction, _directionArgs.Value.ProjectileCounts);

            _endAttackEvent.Invoke();
        }

        private void Shoot(Vector3 direction, int projectileCounts)
        {
            Vector2 perpindicular = Vector2.Perpendicular(new Vector2(direction.x, direction.z)).normalized;

            float offesetBetweenProjectiles = 0.6f;

            for (int i = 0; i < projectileCounts; i++)
            {
                Vector2 offset = perpindicular * (-offesetBetweenProjectiles / 2f * (projectileCounts - 1) + i * offesetBetweenProjectiles);
                Vector3 position = new Vector3(_shootPoint.position.x + offset.x, _shootPoint.position.y, _shootPoint.position.z + offset.y);

                _entitiesFactory.CreateProjectile(position, direction, _damage.Value, _shooterEntity);
            }
        }
    }
}
