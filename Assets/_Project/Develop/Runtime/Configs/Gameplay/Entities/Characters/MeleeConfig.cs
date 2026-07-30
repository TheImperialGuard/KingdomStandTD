using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Characters/NewMeleeConfig", fileName = "MeleeConfig")]
    public class MeleeConfig : CharacterConfig
    {
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)] public float RotateSpeed { get; private set; } = 900;
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
        [field: SerializeField] public DamageTypes DamageResistanceType { get; private set; } = DamageTypes.None;
        [field: SerializeField, Range(0, 1)] public float DamageResistanceIndex { get; private set; } = 0;
        [field: SerializeField, Min(0)] public int DamageOnFinishPath { get; private set; } = 1;
        [field: SerializeField, Min(0)] public int GoldOnDeath { get; private set; } = 10;
    }
}
