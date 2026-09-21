using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class MoveToTargetState : State, IUpdatableState
    {
        private ReactiveVariable<Entity> _currentTarget;

        private ReactiveVariable<Vector3> _movementDirection;

        private Transform _transform;

        public MoveToTargetState(Entity entity)
        {
            _currentTarget = entity.CurrentTarget;
            _movementDirection = entity.MoveDirection;
            _transform = entity.Transform;
        }

        public void Update(float deltaTime)
        {
            Vector3 targetPosition;

            if (TryGetTargetPosition(out targetPosition) == false)
                return;

            _movementDirection.Value = (targetPosition - _transform.position).normalized;
        }

        private bool TryGetTargetPosition(out Vector3 position)
        {
            position = Vector3.zero;

            Entity target = _currentTarget.Value;

            if (target == null)
                return false;

            Transform aimingPoint;

            if (target.TryGetAimingPoint(out aimingPoint) && aimingPoint != null)
            {
                position = aimingPoint.position;
                return true;
            }

            if (target.Transform == null)
                return false;

            position = target.Transform.position;
            return true;
        }
    }
}
