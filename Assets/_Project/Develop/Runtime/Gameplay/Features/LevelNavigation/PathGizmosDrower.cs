using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Editor
{
    public class PathGizmosDrower : MonoBehaviour
    {
        [field: SerializeField] public Color PathColor { get; private set; } = Color.blue;

        void OnDrawGizmosSelected()
        {
            RoadPath roadPath = GetComponent<RoadPath>();

            if (roadPath.Waypoints == null || roadPath.Waypoints.Count < 2)
                return;

            Gizmos.color = PathColor;

            for (int j = 0; j < roadPath.Waypoints.Count - 1; j++)
            {
                Gizmos.DrawLine(roadPath.Waypoints[j].transform.position, roadPath.Waypoints[j + 1].transform.position);
            }
        }
    }
}
