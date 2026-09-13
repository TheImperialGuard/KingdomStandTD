using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewShootingTowerConfig", fileName = "ShootingTowerConfig")]
    public class ShootingTowerConfig : EntityConfig
    {
        [SerializeField] private DamageTypes _damageType = DamageTypes.Physic;

        [field: SerializeField] public TowerTypes TowerType { get; private set; }

        [field: SerializeField] public string PrefabPath { get; private set; }

        [field: SerializeField] public string TowerName { get; private set; }
        [field: SerializeField] public string TowerDesc { get; private set; }

        [field: SerializeField, Min(0)] public int Cost { get; private set; } = 70;

        [field: SerializeField, Min(0)] public float AttackRange { get; private set; } = 3f;

        [field: SerializeField, Min(0)] public float AttackPerSecond { get; private set; } = 1f;

        [field: SerializeField, Range(0, 1f)] public float AttackProcessTime { get; private set; } = 1f;
        [field: SerializeField, Range(0, 1f)] public float AttackDelayTime { get; private set; } = 0.75f;
        [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; } = 1f;

        [field: SerializeField] public ProjectilesTypes Projectile { get; private set; }
        [field: SerializeField, Min(0)] public float AttackDamage { get; private set; } = 2f;
        [field: SerializeField, Min(0)] public float ProjectileSpeed { get; private set; } = 5f;

        public DamageTypes DamageType => _damageType;

        private void OnValidate()
        {
            if (_damageType == DamageTypes.None)
            {
                _damageType = DamageTypes.Physic;

                Debug.LogWarning("Attack damage type can not be None");
            }

            if (AttackDelayTime > AttackProcessTime)
            {
                AttackProcessTime = AttackDelayTime;
            }
        }
    }
}
