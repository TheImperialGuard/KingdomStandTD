using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewTowersListConfig", fileName = "TowersListConfig")]
    public class TowersListConfig : ScriptableObject
    {
        [SerializeField] private List<TowerUpgradeTreeConfig> _upgradeTreeConfigs;

        public ShootingTowerConfig GetBy(TowerTypes towerType, int towerLevel)
        {
            TowerUpgradeTreeConfig towerTree = _upgradeTreeConfigs.First((tree) => tree.TowerType == towerType);

            if (towerTree == null)
                throw new ArgumentException($"Not found upgrades config for tower type: {towerType}");

            ShootingTowerConfig towerConfig = towerLevel switch
            {
                1 => towerTree.FirstLevel,
                2 => towerTree.SecondLevel,
                3 => towerTree.ThirdLevel,
                4 => towerTree.FourthLevel,
                5 => towerTree.FourthLevelAlt,
                _ => throw new ArgumentOutOfRangeException($"Not support for tower level: {towerLevel} type: {towerType}"),
            };

            return towerConfig;
        }
    }
}