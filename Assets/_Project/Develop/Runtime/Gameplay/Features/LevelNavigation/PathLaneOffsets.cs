using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    // Смещение задаётся в локальных осях вейпоинта: ось X направлена поперёк дороги.
    public static class PathLaneOffsets
    {
        public const float LANE_OFFSET = 0.5f;

        public static Vector3 ToLocalOffset(PathLane lane)
        {
            switch (lane)
            {
                case PathLane.Center:
                    return Vector3.zero;

                case PathLane.Left:
                    return Vector3.left * LANE_OFFSET;

                case PathLane.Right:
                    return Vector3.right * LANE_OFFSET;

                default:
                    throw new ArgumentOutOfRangeException(nameof(lane), lane, null);
            }
        }
    }
}
