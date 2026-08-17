using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewMagicTowerConfig", fileName = "MagicTowerConfig")]
    public class MagicTowerConfig : TowerConfig
    {
        [field: SerializeField, Min(0)] public float ProjectileSpeed { get; private set; } = 3f;
        [field: SerializeField] public ProjectilesTypes Projectile { get; private set; } = ProjectilesTypes.Magic;
    }
}
