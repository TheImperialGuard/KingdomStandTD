using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    public class RoadPath : MonoBehaviour
    {
        [SerializeField] private List<Waypoint> _waypoints;

        public IReadOnlyList<Waypoint> Waypoints => _waypoints;
    }
}
