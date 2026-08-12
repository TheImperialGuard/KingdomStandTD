using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/PoisonedAttackAbilityConfig", fileName = "PoisonedAttackAbilityConfig")]
    public class PoisonedAttackAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<float> _poisonDamagePerSecondByLevel;
        [SerializeField] private List<float> _poisonEffectTimeByLevel;

        public float GetDamagePerSecondByLevel(int level) => _poisonDamagePerSecondByLevel[level - 1];
        public float GetEffectTimeByLevel(int level) => _poisonEffectTimeByLevel[level - 1];
    }
}
