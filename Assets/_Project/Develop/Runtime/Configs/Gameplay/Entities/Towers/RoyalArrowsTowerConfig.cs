using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewRoyalArrowsTowerConfig", fileName = "RoyalArrowsTowerConfig")]
    public class RoyalArrowsTowerConfig : TowerConfig
    {
        [field: SerializeField, Min(0)] public float ProjectileSpeed { get; private set; } = 6f;

        [SerializeField] private List<AbilityConfig> _firstAbility;
        [SerializeField] private List<AbilityConfig> _secondAbility;

        public IReadOnlyList<AbilityConfig> FirstAbility => _firstAbility;
        public IReadOnlyList<AbilityConfig> SecondAbility => _secondAbility;
    }
}
