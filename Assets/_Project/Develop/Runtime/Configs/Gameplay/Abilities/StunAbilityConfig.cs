using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/StunAbilityConfig", fileName = "StunAbilityConfig")]
    public class StunAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<float> _stunTimePerLevel;

        [field: SerializeField, Min(0)] public float Radius = 1f;

        [field: SerializeField, Min(0)] public float InitialTime;
        [field: SerializeField, Min(0)] public float Delay;
        [field: SerializeField, Min(0)] public float Cooldown;

        public float GetStunTimeBy(int level) => _stunTimePerLevel[level - 1];
    }
}
