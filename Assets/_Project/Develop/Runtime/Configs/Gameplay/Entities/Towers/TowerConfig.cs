using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    public abstract class TowerConfig : EntityConfig
    {
        [SerializeField] private DamageTypes _damageType = DamageTypes.Physic;

        [field: SerializeField] public string PrefabPath { get; private set; }

        [field: SerializeField] public string TowerName { get; private set; }

        [field: SerializeField, Min(0)] public int Cost { get; private set; } = 70;

        [field: SerializeField, Min(0)] public float AttackRange { get; private set; } = 3f;

        [field: SerializeField, Min(0)] public float AttackPerSecond { get; private set; } = 1f;

        [field: SerializeField, Min(0)] public float AttackDamage { get; private set; } = 2f;

        public DamageTypes DamageType => _damageType;

        private void OnValidate()
        {
            if (_damageType == DamageTypes.None)
            {
                _damageType = DamageTypes.Physic;

                Debug.LogWarning("Attack damage type can not be None");
            }
        }
    }
}
