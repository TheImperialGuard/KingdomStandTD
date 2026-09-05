using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Projectiles/CannonballProjectileConfig", fileName = "CannonballProjectileConfig")]
    public class CannonballProjectileConfig : ProjectileConfig
    {
        [field: SerializeField, Min(0.1f)] public float DamageAreaRadius { get; private set; } = 0.5f;
    }
}
