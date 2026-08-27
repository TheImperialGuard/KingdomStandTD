using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    public class PathDistanceCalcSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _source;

        private List<Waypoint> _path;
        private List<Waypoint> _reachedWaypoints;

        private ReactiveVariable<float> _currentPathDistance;
        private ReactiveVariable<float> _totalPathDistance;
        private ReactiveVariable<float> _traveledPathDistance;

        private bool _isInit;

        public void OnInit(Entity entity)
        {
            _source = entity.Transform;

            _path = entity.Waypoints;
            _reachedWaypoints = entity.ReachedWaypoints;

            _currentPathDistance = entity.CurrentPathDistance;
            _totalPathDistance = entity.TotalPathDistance;
            _traveledPathDistance = entity.TraveledPathDistance;

            _totalPathDistance.Value = CalculateTotalDistance(_path, _source);

            _isInit = true;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isInit == false)
                return;

            _currentPathDistance.Value = CalculateTotalDistance(_path, _source);
            _traveledPathDistance.Value = _totalPathDistance.Value - _currentPathDistance.Value;
        }

        private float CalculateTotalDistance(List<Waypoint> path, Transform startPoint)
        {
            float totalDistance = 0f;

            Vector3 startPos = startPoint.position;

            foreach (Waypoint waypoint in path)
            {
                if (_reachedWaypoints.Contains(waypoint))
                    continue;

                Vector3 direction = waypoint.transform.position - startPos;

                totalDistance += direction.magnitude;
                startPos = waypoint.transform.position;
            }

            return totalDistance;
        }
    }
}
