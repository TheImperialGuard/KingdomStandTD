using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    public class WaypointsNavigationSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float ReachDistance = 0.05f;

        private Transform _soure;

        private List<Waypoint> _waypoints;
        private List<Waypoint> _reachedWaypoints;
        private ReactiveVariable<Vector3> _waypointsOffset;

        private ReactiveVariable<Waypoint> _currentWaypoint;
        private ReactiveVariable<Vector3> _currentLanePoint;
        private Vector3 _previousLanePoint;

        private ReactiveVariable<bool> _isPathFinished;

        public void OnInit(Entity entity)
        {
            _soure = entity.Transform;

            _waypoints = entity.Waypoints;
            _reachedWaypoints = entity.ReachedWaypoints;
            _waypointsOffset = entity.WaypointsOffset;

            _currentWaypoint = entity.CurrentWaypoint;
            _currentLanePoint = entity.CurrentLanePoint;

            // Первый отрезок полосы начинается там, где враг появился, — так же и у призванных скелетов.
            _previousLanePoint = _soure.position;
            SetCurrentWaypoint(_waypoints[0]);

            _isPathFinished = entity.IsPathFinished;
            _isPathFinished.Value = false;
        }

        public void OnUpdate(float deltaTime)
        {
            if (IsCurrentWaypointReached() == false)
                return;

            _reachedWaypoints.Add(_currentWaypoint.Value);

            SwitchWaypoint();
        }

        // На долгом кадре шаг врага больше окна ReachDistance: он перескакивает точку, не попав в окно.
        // Поэтому точка засчитана и тогда, когда враг уже прошёл плоскость через неё поперёк отрезка полосы.
        private bool IsCurrentWaypointReached()
        {
            Vector3 toLanePoint = _currentLanePoint.Value - _soure.position;

            if (toLanePoint.sqrMagnitude <= ReachDistance * ReachDistance)
                return true;

            Vector3 segment = _currentLanePoint.Value - _previousLanePoint;

            return Vector3.Dot(toLanePoint, segment) < 0;
        }

        private void SwitchWaypoint()
        {
            if (_reachedWaypoints.Count == _waypoints.Count)
            {
                _isPathFinished.Value = true;
                return;
            }

            _previousLanePoint = _currentLanePoint.Value;
            SetCurrentWaypoint(_waypoints[_reachedWaypoints.Count]);
        }

        private void SetCurrentWaypoint(Waypoint waypoint)
        {
            Transform waypointTransform = waypoint.transform;

            _currentWaypoint.Value = waypoint;
            _currentLanePoint.Value = waypointTransform.position + waypointTransform.TransformVector(_waypointsOffset.Value);
        }
    }
}
