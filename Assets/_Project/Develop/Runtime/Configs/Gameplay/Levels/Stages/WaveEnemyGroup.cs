using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages
{
    [Serializable]
    public class WaveEnemyGroup
    {
        [field: SerializeField] public CharacterConfig EnemyConfig { get; private set; }
        [field: SerializeField, Min(1)] public int Count { get; private set; } = 1;
    }
}
