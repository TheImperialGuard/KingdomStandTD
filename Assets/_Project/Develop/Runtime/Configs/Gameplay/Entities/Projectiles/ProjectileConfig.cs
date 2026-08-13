using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles
{
    public abstract class ProjectileConfig : EntityConfig
    {
        [field: SerializeField] public ProjectilesTypes Type { get; private set; }
        [field: SerializeField] public string PrefabPath { get; private set; }
    }
}
