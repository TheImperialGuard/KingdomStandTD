using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/NewTowers/HighMagicTowerConfig", fileName = "HighMagicTowerConfig")]
    public class HighMagicTowerConfig : TowerConfig, IHasAbilitiesTowerConfig
    {
        [field: SerializeField, Min(0)] public float ProjectileSpeed { get; private set; } = 6f;
        [field: SerializeField] public ProjectilesTypes Projectile { get; private set; } = ProjectilesTypes.Magic;

        [SerializeField] private List<AbilityConfig> _firstAbility;
        [SerializeField] private List<AbilityConfig> _secondAbility;

        public IReadOnlyList<AbilityConfig> FirstAbilityGroup => _firstAbility;
        public IReadOnlyList<AbilityConfig> SecondAbilityGroup => _secondAbility;
    }
}
