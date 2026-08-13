using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewArrowsTowerConfig", fileName = "ArrowsTowerConfig")]
    public class ArrowsTowerConfig : TowerConfig
    {
        [field: SerializeField, Min(0)] public float ProjectileSpeed { get; private set; } = 5f;
        [field: SerializeField] public ProjectilesTypes Projectile { get; private set; } = ProjectilesTypes.Arrow;
    }
}
