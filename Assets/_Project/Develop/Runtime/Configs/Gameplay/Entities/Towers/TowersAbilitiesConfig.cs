using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewTowerAbilitiesConfig", fileName = "TowerAbilitiesConfig")]
    public class TowersAbilitiesConfig : ScriptableObject
    {
        [SerializeField] private List<AbilityConfig> _firstAbility;
        [SerializeField] private List<AbilityConfig> _secondAbility;

        [field: SerializeField] public TowerTypes Tower;

        public IReadOnlyList<AbilityConfig> FirstAbilityGroup => _firstAbility;
        public IReadOnlyList<AbilityConfig> SecondAbilityGroup => _secondAbility;
    }
}
