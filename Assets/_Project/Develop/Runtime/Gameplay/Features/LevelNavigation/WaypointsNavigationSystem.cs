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
        private Transform _soure;

        private List<Waypoint> _waypoints;
        private List<Waypoint> _reachedWaypoints;
        private ReactiveVariable<Vector3> _waypointsOffset;

        private ReactiveVariable<Waypoint> _currentWaypoint;

        private ReactiveVariable<bool> _isPathFinished;

        public void OnInit(Entity entity)
        {
            _soure = entity.Transform;

            _waypoints = entity.Waypoints;
            _reachedWaypoints = entity.ReachedWaypoints;
            _waypointsOffset = entity.WaypointsOffset;

            _currentWaypoint = entity.CurrentWaypoint;
            _currentWaypoint.Value = _waypoints[0];

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

        private bool IsCurrentWaypointReached()
        {
            Transform waypoint = _currentWaypoint.Value.transform;
            Vector3 waypointPos = _currentWaypoint.Value.transform.position;

            waypointPos += waypoint.TransformVector(_waypointsOffset.Value);

            float distanceToWaypoint = (waypointPos - _soure.position).magnitude;

            return distanceToWaypoint < 0.01f;
        }

        private void SwitchWaypoint()
        {
            if (_reachedWaypoints.Count == _waypoints.Count)
            {
                _isPathFinished.Value = true;
                return;
            }

            _currentWaypoint.Value = _waypoints[_reachedWaypoints.Count];
        }
    }
}
