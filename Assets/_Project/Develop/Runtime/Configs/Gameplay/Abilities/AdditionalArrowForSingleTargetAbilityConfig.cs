using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/AdditionalArrowForSingleTargetAbilityConfig", fileName = "AdditionalArrowForSingleTargetAbilityConfig")]
    public class AdditionalArrowForSingleTargetAbilityConfig : AbilityConfig
    {
        [SerializeField] private int _additionalArrowPerLevel;

        public int GetAdditionsArrowsBy(int level) => level * _additionalArrowPerLevel;
    }
}
