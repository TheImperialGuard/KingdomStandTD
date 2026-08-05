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
        private ReactiveVariable<Vector3> _waypointsOffset;

        public WaypointsMovementState(Entity entity)
        {
            _currentWaypoint = entity.CurrentWaypoint;
            _movementDirection = entity.MoveDirection;
            _rotationDirection = entity.RotationDirection;
            _transform = entity.Transform;

            _currentDirection = Vector3.zero;

            if (entity.TryGetWaypointsOffset(out ReactiveVariable<Vector3> waypointsOffset))
                _waypointsOffset = waypointsOffset;
        }

        public void Update(float deltaTime)
        {
            if (_currentWaypoint == null)
                return;

            Transform waypoint = _currentWaypoint.Value.transform;
            Vector3 waypointPos = waypoint.position;

            waypointPos += waypoint.TransformVector(_waypointsOffset.Value);

            _currentDirection = (waypointPos - _transform.position).normalized;

            _movementDirection.Value = _currentDirection;
            _rotationDirection.Value = _currentDirection;
        }
    }
}
