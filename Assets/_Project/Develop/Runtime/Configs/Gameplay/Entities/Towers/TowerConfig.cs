using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    public abstract class TowerConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; }

        [field: SerializeField, Min(0)] public float AttackRange { get; private set; } = 3f;

        [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; } = 1f;

        [field: SerializeField, Min(0)] public float AttackDamage { get; private set; } = 2f;

        [field: SerializeField] public DamageTypes DamageType { get; private set; } = DamageTypes.Physic;
    }
}
