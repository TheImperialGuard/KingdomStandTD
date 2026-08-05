using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class ShootDirectionCalculateSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<InstantShotDirectionArgs> _directionArgs;

        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<float> _projectileSpeed;

        private Transform _shootPoint;

        public void OnInit(Entity entity)
        {
            _directionArgs = entity.InstantShotDirection;
            _currentTarget = entity.CurrentTarget;
            _projectileSpeed = entity.ProjectileSpeed;
            _shootPoint = entity.ShootPoint;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_currentTarget.Value != null)
            {
                Vector3 leadPoint = CalculateLeadPoint();

                Vector3 directionToTarget = (leadPoint - _shootPoint.position).normalized;

                _directionArgs.Value = new(directionToTarget, 1);
            } 
            else
            {
                _directionArgs.Value = null;
            }
        }

        private Vector3 CalculateLeadPoint()
        {
            if (_currentTarget.Value.TryGetAimingPoint(out Transform aimingPoint) == false)
                throw new Exception($"Not found aiming point for target: {_currentTarget.Value.Transform.gameObject.name}");

            Vector3 currentTargetPosition = aimingPoint.position;
            Vector3 currentTargetSpeed = _currentTarget.Value.Rigidbody.linearVelocity;
            Vector3 shootPointPosition = _shootPoint.position;

            float projectileSpeed = _projectileSpeed.Value;

            if (currentTargetSpeed == Vector3.zero)
                return currentTargetPosition;

            float timeToTarget = Vector3.Distance(shootPointPosition, currentTargetPosition) / projectileSpeed;

            Vector3 leadPoint = currentTargetPosition + currentTargetSpeed * timeToTarget;

            return leadPoint;
        }
    }
}
