using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/PoisonedAttackAbilityConfig", fileName = "PoisonedAttackAbilityConfig")]
    public class PoisonedAttackAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<float> _poisonDamage;
        [SerializeField] private List<float> _poisonEffectTimeByLevel;
        [SerializeField] private List<float> _poisonEffectIntervalTimeByLevel;

        public float GetDamageByLevel(int level) => _poisonDamage[level - 1];
        public float GetEffectTimeByLevel(int level) => _poisonEffectTimeByLevel[level - 1];
        public float GetEffectIntervalByLevel(int level) => _poisonEffectIntervalTimeByLevel[level - 1];
    }
}
