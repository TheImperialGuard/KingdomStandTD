using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewTowersAbilitiesListConfig", fileName = "TowersAbilitiesListConfig")]
    public class TowersAbilitiesListConfig : ScriptableObject
    {
        [SerializeField] private List<TowersAbilitiesConfig> _abilitiesConfigs;

        public TowersAbilitiesConfig GetBy(TowerTypes towerType)
        {
            TowersAbilitiesConfig abilitiesConfig = _abilitiesConfigs.First((config) => config.Tower == towerType);

            if (abilitiesConfig == null)
                throw new ArgumentException($"Not found abilities config for tower type: {towerType}");

            return abilitiesConfig;
        }
    }
}