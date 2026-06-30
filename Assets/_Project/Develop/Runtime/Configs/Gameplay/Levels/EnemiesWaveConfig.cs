using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/Waves/NewEnemiesWaveConfig", fileName = "EnemiesWaveConfig")]
    public class EnemiesWaveConfig : ScriptableObject
    {
        [field: SerializeField] public int WaveStage { get; private set; } = 1;
        [field: SerializeField, Min(0)] public float WaveStartDelay { get; private set; } = 0;
        [field: SerializeField, Min(0)] public float WaveDelayBetweenSpawns { get; private set; } = 1;
        [field: SerializeField] public EntityConfig EnemyConfig { get; private set; }
        [field: SerializeField] public int EnemiesCount { get; private set; } = 1;
    }
}
