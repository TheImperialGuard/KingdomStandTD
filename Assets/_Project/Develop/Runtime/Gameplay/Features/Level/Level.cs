using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private List<RoadPath> _roadPaths;
        [SerializeField] private List<TowerTile> _towerTiles;

        private readonly Dictionary<PathNumber, Color> _pathNumberToGizmosColors = new()
        {
            { PathNumber.First, Color.blue },
            { PathNumber.Second, Color.green },
            { PathNumber.Third, Color.red },
        };

        public IReadOnlyList<RoadPath> RoadPaths => _roadPaths;

        void OnDrawGizmos()
        {
            for (int i = 0; i < _roadPaths.Count; i++)
            {
                RoadPath path = _roadPaths[i];

                if (path.Waypoints == null || path.Waypoints.Count < 2)
                    continue;

                Gizmos.color = _pathNumberToGizmosColors[path.Number];

                for (int j = 0; j < _roadPaths[0].Waypoints.Count - 1; j++)
                {
                    Gizmos.DrawLine(path.Waypoints[j].transform.position, path.Waypoints[j + 1].transform.position);
                }
            }
        }
    }
}
