using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private List<EnemiesWavesStageConfig> _enemiesWavesStageConfigs;
        [SerializeField] private List<Transform> _towersPositions;

        public IReadOnlyList<EnemiesWavesStageConfig> EnemiesWavesStageConfigs => _enemiesWavesStageConfigs;
        public IReadOnlyList<Transform> TowersPositions => _towersPositions;
    }
}
