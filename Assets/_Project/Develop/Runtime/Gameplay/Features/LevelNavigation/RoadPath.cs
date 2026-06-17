using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    [Serializable]
    public class RoadPath
    {
        [SerializeField] private PathNumber _number;

        [SerializeField] private List<Waypoint> _waypoints;


        public IReadOnlyList<Waypoint> Waypoints => _waypoints;

        public PathNumber Number => _number;
    }
}
