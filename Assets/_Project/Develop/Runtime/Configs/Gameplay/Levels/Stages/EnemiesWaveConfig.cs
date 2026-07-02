using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [Serializable]
    public class EnemiesWaveConfig
    {
        [field: SerializeField] public EntityConfig EnemyConfig { get; private set; }
        [field: SerializeField] public RoadPath RoadPath { get; private set; }
        [field: SerializeField, Min(0)] public float StartDelay { get; private set; } = 0;
        [field: SerializeField, Min(0.1f)] public float DelayBetweenSpawns { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int EnemiesCount { get; private set; } = 1;
    }
}
