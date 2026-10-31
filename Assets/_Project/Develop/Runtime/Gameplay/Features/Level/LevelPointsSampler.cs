using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    // Случайные точки уровня на земле: на дороге и в поле вне дороги. Дорога — ломаная вейпоинтов, точка на ней выбирается
    // равномерно по длине. Точка вне дороги — случайная точка поля, которая не ближе полуширины дороги к ломаной.
    public class LevelPointsSampler
    {
        // Дорога — ряд тайлов 2 × 2, ломаная вейпоинтов идёт по её середине
        private const float RoadHalfWidth = 1f;
        private const int MaxOffroadAttempts = 20;

        private readonly List<Vector3[]> _roads = new();
        private readonly List<float> _roadsLengths = new();
        private readonly Bounds _fieldBounds;
        private readonly float _groundHeight;

        public LevelPointsSampler(Level level)
        {
            foreach (RoadPath roadPath in level.RoadPaths)
            {
                Vector3[] points = GetPoints(roadPath);

                _roads.Add(points);
                _roadsLengths.Add(GetLength(points));
            }

            _fieldBounds = level.FieldBounds;

            // Земля — на высоте вейпоинтов: по ним ходят враги
            _groundHeight = _roads[0][0].y;
        }

        // Дорога выбирается поровну, место на ней — равномерно по длине
        public Vector3 GetRandomRoadPoint()
        {
            int roadIndex = Random.Range(0, _roads.Count);
            float distance = Random.Range(0f, _roadsLengths[roadIndex]);

            return GetPointAt(_roads[roadIndex], distance);
        }

        public Vector3 GetRandomOffroadPoint()
        {
            Vector3 point = GetRandomFieldPoint();

            for (int attempt = 1; attempt < MaxOffroadAttempts && IsOnRoad(point); attempt++)
                point = GetRandomFieldPoint();

            // Если поле почти целиком занято дорогой, точка может так и остаться на дороге
            return point;
        }

        private Vector3 GetRandomFieldPoint()
        {
            float x = Random.Range(_fieldBounds.min.x, _fieldBounds.max.x);
            float z = Random.Range(_fieldBounds.min.z, _fieldBounds.max.z);

            return new Vector3(x, _groundHeight, z);
        }

        private bool IsOnRoad(Vector3 point)
        {
            foreach (Vector3[] road in _roads)
            {
                for (int i = 1; i < road.Length; i++)
                {
                    if (GetFlatDistanceToSegment(point, road[i - 1], road[i]) < RoadHalfWidth)
                        return true;
                }
            }

            return false;
        }

        private static Vector3[] GetPoints(RoadPath roadPath)
        {
            Vector3[] points = new Vector3[roadPath.Waypoints.Count];

            for (int i = 0; i < points.Length; i++)
                points[i] = roadPath.Waypoints[i].transform.position;

            return points;
        }

        private static float GetLength(Vector3[] points)
        {
            float length = 0f;

            for (int i = 1; i < points.Length; i++)
                length += Vector3.Distance(points[i - 1], points[i]);

            return length;
        }

        private static Vector3 GetPointAt(Vector3[] points, float distance)
        {
            for (int i = 1; i < points.Length; i++)
            {
                float segmentLength = Vector3.Distance(points[i - 1], points[i]);

                if (distance <= segmentLength)
                    return Vector3.Lerp(points[i - 1], points[i], distance / segmentLength);

                distance -= segmentLength;
            }

            return points[points.Length - 1];
        }

        // Расстояние по горизонтали: высоту точки поля и вейпоинтов не сравниваем
        private static float GetFlatDistanceToSegment(Vector3 point, Vector3 start, Vector3 end)
        {
            Vector2 flatPoint = new(point.x, point.z);
            Vector2 flatStart = new(start.x, start.z);
            Vector2 segment = new Vector2(end.x, end.z) - flatStart;

            float segmentSqrLength = segment.sqrMagnitude;

            if (segmentSqrLength == 0f)
                return Vector2.Distance(flatPoint, flatStart);

            float progress = Mathf.Clamp01(Vector2.Dot(flatPoint - flatStart, segment) / segmentSqrLength);

            return Vector2.Distance(flatPoint, flatStart + segment * progress);
        }
    }
}
