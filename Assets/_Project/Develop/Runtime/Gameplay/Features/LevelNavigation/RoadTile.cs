using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    public class RoadTile : MonoBehaviour
    {
        [SerializeField] private List<Transform> _waypoints;

        public IReadOnlyList<Transform> Waypoints => _waypoints;
    }
}
