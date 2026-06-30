using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Characters/NewMeleeConfig", fileName = "MeleeConfig")]
    public class MeleeConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Prefabs/Entities/Characters/Melee";
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)] public float RotateSpeed { get; private set; } = 900;
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
    }
}
