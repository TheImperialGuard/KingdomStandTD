using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay
{
    [Serializable]
    public class RoutePreviewColor
    {
        public RoutePreviewColor(PathNumber pathNumber, Color color)
        {
            PathNumber = pathNumber;
            Color = color;
        }

        [field: SerializeField] public PathNumber PathNumber { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    }
}
