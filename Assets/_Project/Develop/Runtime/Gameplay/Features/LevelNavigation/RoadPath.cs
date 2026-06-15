using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    [Serializable]
    public class RoadPath
    {
        [SerializeField] private PathNumber _number;

        [SerializeField] private Transform _spawner;

        [SerializeField] private List<RoadTile> _tiles;

        public Vector3 SpawnerPosition => _spawner.position;

        public IReadOnlyList<RoadTile> Tiles => _tiles;

        public PathNumber Number => _number;
    }
}
