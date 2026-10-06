using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Маршрут врага по полосе дороги — как в WavesSpawner и WaypointsNavigationSystem: спавн в первом вейпоинте,
    // дальше по прямой к каждому следующему; смещение полосы — в локальных осях каждого вейпоинта. По нему считаются
    // и длины полос в срезах, и линии полос в Scene View.
    public static class RoadLanes
    {
        // Путь целый: от двух вейпоинтов и без пустых.
        public static bool IsComplete(IReadOnlyList<Waypoint> waypoints)
        {
            if (waypoints == null || waypoints.Count < LevelPrefabReader.MIN_WAYPOINTS_COUNT)
                return false;

            foreach (Waypoint waypoint in waypoints)
            {
                if (waypoint == null)
                    return false;
            }

            return true;
        }

        public static Vector3 GetPoint(Waypoint waypoint, PathLane lane)
        {
            Transform waypointTransform = waypoint.transform;

            return waypointTransform.position + waypointTransform.TransformVector(PathLaneOffsets.ToLocalOffset(lane));
        }

        public static float CalculateLength(IReadOnlyList<Waypoint> waypoints, PathLane lane)
        {
            float length = 0f;
            Vector3 previousPoint = GetPoint(waypoints[0], lane);

            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 point = GetPoint(waypoints[i], lane);

                length += Vector3.Distance(previousPoint, point);
                previousPoint = point;
            }

            return length;
        }
    }
}
