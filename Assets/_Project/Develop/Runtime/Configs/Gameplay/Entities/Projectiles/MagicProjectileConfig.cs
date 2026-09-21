using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Projectiles/MagicProjectileConfig", fileName = "MagicProjectileConfig")]
    public class MagicProjectileConfig : ProjectileConfig
    {
        [field: SerializeField, Min(0)] public float SpeedAcceleration { get; private set; } = 0.5f;
        [field: SerializeField, Min(0)] public float MaxSpeed { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float SpawnProcessTime { get; private set; } = 0.5f;
    }
}
