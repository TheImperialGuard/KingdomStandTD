using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.RoutesPreview
{
    // Ломаная по центрам вейпоинтов пути: от тайла спавна до последнего вейпоинта у базы.
    public class RouteLine
    {
        private readonly List<Vector3> _points = new();
        private readonly List<float> _distancesToPoints = new();

        public RouteLine(RoadPath roadPath, float heightOffset)
        {
            Vector3 offset = Vector3.up * heightOffset;

            foreach (Waypoint waypoint in roadPath.Waypoints)
                AddPoint(waypoint.transform.position + offset);
        }

        public float Length { get; private set; }

        public Vector3 GetPoint(float distance)
        {
            if (distance <= 0)
                return _points[0];

            for (int i = 1; i < _points.Count; i++)
            {
                if (distance > _distancesToPoints[i])
                    continue;

                float segmentStart = _distancesToPoints[i - 1];
                float segmentLength = _distancesToPoints[i] - segmentStart;
                float progress = (distance - segmentStart) / segmentLength;

                return Vector3.Lerp(_points[i - 1], _points[i], progress);
            }

            return _points[_points.Count - 1];
        }

        public Vector3 GetDirection(float distance, float lookAheadDistance)
        {
            // В конце пути смотреть вперёд некуда, поэтому направление берётся по участку перед концом
            float lookFrom = Mathf.Min(distance, Length - lookAheadDistance);

            return GetPoint(lookFrom + lookAheadDistance) - GetPoint(lookFrom);
        }

        private void AddPoint(Vector3 point)
        {
            if (_points.Count == 0)
            {
                _points.Add(point);
                _distancesToPoints.Add(0);
                return;
            }

            float segmentLength = Vector3.Distance(_points[_points.Count - 1], point);

            // Совпадающие вейпоинты дают отрезок нулевой длины и деление на ноль в GetPoint
            if (segmentLength <= 0)
                return;

            Length += segmentLength;

            _points.Add(point);
            _distancesToPoints.Add(Length);
        }
    }
}
