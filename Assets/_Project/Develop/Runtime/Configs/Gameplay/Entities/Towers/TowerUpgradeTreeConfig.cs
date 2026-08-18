using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/TowersTrees/TowerUpgradeTreeConfig", fileName = "TowerUpgradeTreeConfig")]
    public class TowerUpgradeTreeConfig : ScriptableObject
    {
        [field: SerializeField] public TowerTypes TowerType { get; private set; }
        [field: SerializeField] public ShootingTowerConfig FirstLevel { get; private set; }
        [field: SerializeField] public ShootingTowerConfig SecondLevel { get; private set; }
        [field: SerializeField] public ShootingTowerConfig ThirdLevel { get; private set; }
        [field: SerializeField] public ShootingTowerConfig FourthLevel { get; private set; }
        [field: SerializeField] public ShootingTowerConfig FourthLevelAlt { get; private set; }
    }
}
