using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/AttackStunAbilityConfig", fileName = "AttackStunAbilityConfig")]
    public class AttackStunAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<float> _stunTimePerLevel;

        public float GetStunTimeBy(int level) => _stunTimePerLevel[level - 1];
    }
}
