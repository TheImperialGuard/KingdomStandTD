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

        // Длина маршрута от каждого вейпоинта до последнего. Вейпоинты неподвижны, поэтому хвосты считаются один раз.
        private float[] _tailDistances;

        private ReactiveVariable<float> _currentPathDistance;
        private ReactiveVariable<float> _totalPathDistance;
        private ReactiveVariable<float> _traveledPathDistance;

        public void OnInit(Entity entity)
        {
            _source = entity.Transform;

            _path = entity.Waypoints;
            _reachedWaypoints = entity.ReachedWaypoints;

            _currentPathDistance = entity.CurrentPathDistance;
            _totalPathDistance = entity.TotalPathDistance;
            _traveledPathDistance = entity.TraveledPathDistance;

            _tailDistances = CalculateTailDistances(_path);

            _totalPathDistance.Value = CalculateRemainingDistance();
        }

        public void OnUpdate(float deltaTime)
        {
            _currentPathDistance.Value = CalculateRemainingDistance();
            _traveledPathDistance.Value = _totalPathDistance.Value - _currentPathDistance.Value;
        }

        private static float[] CalculateTailDistances(List<Waypoint> path)
        {
            float[] tailDistances = new float[path.Count];

            for (int i = path.Count - 2; i >= 0; i--)
            {
                Vector3 segment = path[i + 1].transform.position - path[i].transform.position;

                tailDistances[i] = tailDistances[i + 1] + segment.magnitude;
            }

            return tailDistances;
        }

        // Пройденные вейпоинты — всегда начало маршрута: навигация засчитывает их по порядку.
        private float CalculateRemainingDistance()
        {
            int nextWaypointIndex = _reachedWaypoints.Count;

            if (nextWaypointIndex >= _path.Count)
                return 0f;

            Vector3 toNextWaypoint = _path[nextWaypointIndex].transform.position - _source.position;

            return toNextWaypoint.magnitude + _tailDistances[nextWaypointIndex];
        }
    }
}
