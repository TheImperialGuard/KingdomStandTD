using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/AdditionalArrowForSingleTargetAbilityConfig", fileName = "AdditionalArrowForSingleTargetAbilityConfig")]
    public class AdditionalArrowForSingleTargetAbilityConfig : AbilityConfig
    {
        [SerializeField] private List<int> _levelsCosts;

        [SerializeField] private int _additionalArrowPerLevel;

        public override int MaxLevel => _levelsCosts.Count;

        public int GetAdditionsArrowsBy(int level) => level * _additionalArrowPerLevel;
    }
}
