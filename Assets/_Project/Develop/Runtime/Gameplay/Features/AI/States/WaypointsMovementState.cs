using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class WaypointsMovementState : State, IUpdatableState
    {
        private ReactiveVariable<Waypoint> _currentWaypoint;

        private ReactiveVariable<Vector3> _movementDirection;
        private ReactiveVariable<Vector3> _rotationDirection;

        private Transform _transform;

        private Vector3 _currentDirection;

        public WaypointsMovementState(Entity entity)
        {
            _currentWaypoint = entity.CurrentWaypoint;
            _movementDirection = entity.MoveDirection;
            _rotationDirection = entity.RotationDirection;
            _transform = entity.Transform;

            _currentDirection = Vector3.zero;
        }

        public void Update(float deltaTime)
        {
            if (_currentWaypoint != null)
                _currentDirection = (_currentWaypoint.Value.transform.position - _transform.position).normalized;

            _movementDirection.Value = _currentDirection;
            _rotationDirection.Value = _currentDirection;
        }
    }
}
