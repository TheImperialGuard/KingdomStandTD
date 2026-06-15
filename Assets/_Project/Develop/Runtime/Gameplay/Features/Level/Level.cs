using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private List<RoadPath> _roadPaths;
    }
}
