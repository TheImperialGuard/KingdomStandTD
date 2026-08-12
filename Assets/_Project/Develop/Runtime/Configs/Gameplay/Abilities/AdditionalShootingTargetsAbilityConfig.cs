using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/AdditionalShootingTargetsAbilityConfig", fileName = "AdditionalShootingTargetsAbilityConfig")]
    public class AdditionalShootingTargetsAbilityConfig : AbilityConfig
    {
        [SerializeField] private int _additionalTargetPerLevel;

        public int GetAdditionsTargetsBy(int level) => level * _additionalTargetPerLevel;
    }
}
