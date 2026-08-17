using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/OneShotAttackAbility", fileName = "OneShotAttackAbility")]
    public class OneShotAttackAbility : AbilityConfig
    {
        [SerializeField] private List<float> _cooldownPerLevel;

        public float GetCooldownBy(int level) => _cooldownPerLevel[level - 1];
    }
}
