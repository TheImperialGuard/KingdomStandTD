using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [Serializable]
    public class EnemiesWaveConfig
    {
        [SerializeField] private List<WaveEnemyGroup> _enemyGroups = new();
        [SerializeField, TextArea] private string _note;

        [field: SerializeField, Min(1)] public int PatternRepeats { get; private set; } = 1;
        [field: SerializeField] public PathNumber PathNumber { get; private set; }
        [field: SerializeField] public PathLane Lane { get; private set; }
        [field: SerializeField, Min(0)] public float StartDelay { get; private set; } = 0;
        [field: SerializeField, Min(0.1f)] public float DelayBetweenSpawns { get; private set; } = 1;

        public IReadOnlyList<WaveEnemyGroup> EnemyGroups => _enemyGroups;
        public string Note => _note;
    }
}
