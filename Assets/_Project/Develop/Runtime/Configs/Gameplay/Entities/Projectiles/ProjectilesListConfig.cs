using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Projectiles/ProjectilesListConfig", fileName = "ProjectilesListConfig")]
    public class ProjectilesListConfig : ScriptableObject
    {
        [SerializeField] private List<ProjectileConfig> _projectilesConfigs;

        public ProjectileConfig GetProjectileConfigBy(ProjectilesTypes type) 
            => _projectilesConfigs.First(config => config.Type == type);
    }
}
