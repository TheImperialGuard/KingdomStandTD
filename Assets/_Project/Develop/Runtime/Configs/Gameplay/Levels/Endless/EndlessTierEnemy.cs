using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Враг тира и волна, начиная с которой генератор может взять его в волну.
    [Serializable]
    public class EndlessTierEnemy
    {
        [field: SerializeField] public MeleeConfig EnemyConfig { get; private set; }
        [field: SerializeField, Min(1)] public int FirstWave { get; private set; } = 1;
    }
}
