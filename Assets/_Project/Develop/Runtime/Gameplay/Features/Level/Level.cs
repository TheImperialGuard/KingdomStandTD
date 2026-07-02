using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private List<EnemiesWavesStageConfig> _enemiesWavesStageConfigs;
        [SerializeField] private List<TowerTile> _towerTiles;

        public IReadOnlyList<EnemiesWavesStageConfig> EnemiesWavesStageConfigs => _enemiesWavesStageConfigs;
        public IReadOnlyList<TowerTile> TowerTiles => _towerTiles;
    }
}
