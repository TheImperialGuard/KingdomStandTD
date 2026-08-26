using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/OneShotAttackAbility", fileName = "OneShotAttackAbility")]
    public class OneShotAttackAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<float> _cooldownPerLevel;

        [field: SerializeField] public ProjectilesTypes ProjectileType { get; private set; } = ProjectilesTypes.MagicTrail;

        [field: SerializeField, Min(0)] public float InitialTime;
        [field: SerializeField, Min(0)] public float Delay;

        public float GetCooldownBy(int level) => _cooldownPerLevel[level - 1];
    }
}
