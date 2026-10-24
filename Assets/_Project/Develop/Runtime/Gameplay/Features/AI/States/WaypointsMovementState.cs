using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class WaypointsMovementState : State, IUpdatableState
    {
        private ReactiveVariable<Vector3> _currentLanePoint;

        private ReactiveVariable<Vector3> _movementDirection;
        private ReactiveVariable<Vector3> _rotationDirection;

        private Transform _transform;

        private Vector3 _currentDirection;

        public WaypointsMovementState(Entity entity)
        {
            _currentLanePoint = entity.CurrentLanePoint;
            _movementDirection = entity.MoveDirection;
            _rotationDirection = entity.RotationDirection;
            _transform = entity.Transform;

            _currentDirection = Vector3.zero;
        }

        public void Update(float deltaTime)
        {
            _currentDirection = (_currentLanePoint.Value - _transform.position).normalized;

            _movementDirection.Value = _currentDirection;
            _rotationDirection.Value = _currentDirection;
        }
    }
}
