using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Читает префаб уровня так же, как игра: Resources.Load по LevelConfig.PrefabPath.
    public static class LevelPrefabReader
    {
        public const int MIN_WAYPOINTS_COUNT = 2;

        public static LevelPrefabInfo Read(LevelConfig config)
        {
            LevelPrefabInfo info = new();

            if (string.IsNullOrWhiteSpace(config.PrefabPath))
            {
                info.Status = LevelPrefabStatus.PathMissing;
                return info;
            }

            GameObject prefab = Resources.Load<GameObject>(config.PrefabPath);

            if (prefab == null)
            {
                info.Status = LevelPrefabStatus.PrefabMissing;
                return info;
            }

            info.AssetPath = AssetDatabase.GetAssetPath(prefab);

            if (prefab.TryGetComponent(out Level level) == false)
            {
                info.Status = LevelPrefabStatus.LevelComponentMissing;
                return info;
            }

            info.Status = LevelPrefabStatus.Found;

            ReadTowerSlots(level, info);

            if (level.RoadPaths == null)
                return info;

            foreach (RoadPath roadPath in level.RoadPaths)
            {
                if (roadPath == null)
                {
                    info.MissingRoadsCount++;
                    continue;
                }

                info.Roads.Add(ReadRoad(roadPath));
            }

            return info;
        }

        private static void ReadTowerSlots(Level level, LevelPrefabInfo info)
        {
            if (level.TowersPositions == null)
                return;

            foreach (Transform towerPosition in level.TowersPositions)
            {
                if (towerPosition == null)
                    info.MissingTowerSlotsCount++;
                else
                    info.TowerSlotsCount++;
            }
        }

        private static LevelRoad ReadRoad(RoadPath roadPath)
        {
            LevelRoad road = new()
            {
                Number = roadPath.Number,
                Name = roadPath.name,
            };

            IReadOnlyList<Waypoint> waypoints = roadPath.Waypoints;

            if (waypoints == null)
                return road;

            road.WaypointsCount = waypoints.Count;

            foreach (Waypoint waypoint in waypoints)
            {
                if (waypoint == null)
                    road.MissingWaypointsCount++;
            }

            road.HasValidPath = road.WaypointsCount >= MIN_WAYPOINTS_COUNT && road.MissingWaypointsCount == 0;

            if (road.HasValidPath == false)
                return road;

            foreach (PathLane lane in Enum.GetValues(typeof(PathLane)))
                road.Lengths[lane] = CalculateLength(waypoints, PathLaneOffsets.ToLocalOffset(lane));

            return road;
        }

        // Повторяет маршрут врага из WavesSpawner и WaypointsNavigationSystem: спавн в первом вейпоинте,
        // дальше по прямой к каждому следующему; смещение полосы — в локальных осях каждого вейпоинта.
        private static float CalculateLength(IReadOnlyList<Waypoint> waypoints, Vector3 localOffset)
        {
            float length = 0f;
            Vector3 previousPoint = GetLanePoint(waypoints[0], localOffset);

            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 point = GetLanePoint(waypoints[i], localOffset);

                length += Vector3.Distance(previousPoint, point);
                previousPoint = point;
            }

            return length;
        }

        private static Vector3 GetLanePoint(Waypoint waypoint, Vector3 localOffset)
        {
            Transform waypointTransform = waypoint.transform;

            return waypointTransform.position + waypointTransform.TransformVector(localOffset);
        }
    }
}
