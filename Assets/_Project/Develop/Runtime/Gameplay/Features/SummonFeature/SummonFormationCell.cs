using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SummonFeature
{
    // Клетка строя: где поднимается призванный, с какого вейпоинта пути призывающего начинает маршрут и какой полосой идёт.
    public readonly struct SummonFormationCell
    {
        public SummonFormationCell(Vector3 position, int firstWaypointIndex, PathLane lane)
        {
            Position = position;
            FirstWaypointIndex = firstWaypointIndex;
            Lane = lane;
        }

        public Vector3 Position { get; }

        public int FirstWaypointIndex { get; }

        public PathLane Lane { get; }
    }
}
