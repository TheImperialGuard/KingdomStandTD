using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    public class WaypointsNavigationSystem : IInitializableSystem, IUpdatableSystem
    {
        private Buffer<Collider> _contacts;

        private List<Waypoint> _waypoints;
        private List<Waypoint> _reachedWaypoints;

        private ReactiveVariable<Waypoint> _currentWaypoint;

        private ReactiveVariable<bool> _isPathFinished;

        public void OnInit(Entity entity)
        {
            _contacts = entity.ContactCollidersBuffer;

            _waypoints = entity.Waypoints;
            _reachedWaypoints = entity.ReachedWaypoints;

            _currentWaypoint = entity.CurrentWaypoint;
            _currentWaypoint.Value = _waypoints[0];

            _isPathFinished = entity.IsPathFinished;
            _isPathFinished.Value = false;
        }

        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                Collider collider = _contacts.Items[i];

                if (collider.TryGetComponent(out Waypoint reachedWaypoint))
                {
                    if (_waypoints.Contains(reachedWaypoint))
                    {
                        _reachedWaypoints.Add(reachedWaypoint);
                        SwitchWaypoint();
                    }    
                }
            }
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
