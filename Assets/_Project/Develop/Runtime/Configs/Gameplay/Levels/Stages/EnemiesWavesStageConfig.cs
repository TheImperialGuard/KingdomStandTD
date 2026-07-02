using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [Serializable]
    public class EnemiesWavesStageConfig : StageConfig
    {
        [SerializeField] private List<EnemiesWaveConfig> _enemiesWaveConfigs;

        [field: SerializeField, Min(0)] public float StageTime { get; private set; } = 30f;
        [field: SerializeField, Min(0)] public float TimeToSkipStage { get; private set; } = 15f;

        public IReadOnlyList<EnemiesWaveConfig> EnemiesWaveConfigs;
    }
}
