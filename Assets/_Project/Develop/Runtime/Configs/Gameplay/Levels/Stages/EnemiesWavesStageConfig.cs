using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [Serializable]
    public class EnemiesWavesStageConfig : StageConfig
    {
        [SerializeField] private List<EnemiesWaveConfig> _enemiesWaveConfigs;
        [SerializeField, TextArea] private string _note;

        // Для сериализации Unity
        public EnemiesWavesStageConfig()
        {
        }

        // Стейдж, собранный в коде, — например, волна бесконечного режима
        public EnemiesWavesStageConfig(List<EnemiesWaveConfig> enemiesWaveConfigs, float stageTime, float timeToSkipStage)
        {
            _enemiesWaveConfigs = enemiesWaveConfigs;
            StageTime = stageTime;
            TimeToSkipStage = timeToSkipStage;
        }

        [field: SerializeField, Min(0)] public float StageTime { get; private set; } = 30f;
        [field: SerializeField, Min(0)] public float TimeToSkipStage { get; private set; } = 15f;

        public IReadOnlyList<EnemiesWaveConfig> EnemiesWaveConfigs => _enemiesWaveConfigs;
        public string Note => _note;
    }
}
