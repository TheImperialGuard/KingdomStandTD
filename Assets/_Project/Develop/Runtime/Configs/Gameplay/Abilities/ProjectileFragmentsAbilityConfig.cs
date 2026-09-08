using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/ProjectileFragmentsAbilityConfig", fileName = "ProjectileFragmentsAbilityConfig")]
    public class ProjectileFragmentsAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<float> _fragmentDamagePerLevel;

        [field: SerializeField] public float FragmentsCount { get; private set; } = 3f;

        public float GetFragmentsDamageBy(int level) => _fragmentDamagePerLevel[level - 1];
    }
}
