using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/TowersTrees/TowerUpgradeTreeConfig", fileName = "TowerUpgradeTreeConfig")]
    public class TowerUpgradeTreeConfig : ScriptableObject
    {
        [field: SerializeField] public TowerTypes TowerType { get; private set; }
        [field: SerializeField] public TowerConfig FirstLevel { get; private set; }
        [field: SerializeField] public TowerConfig SecondLevel { get; private set; }
        [field: SerializeField] public TowerConfig ThirdLevel { get; private set; }
        [field: SerializeField] public TowerConfig FourthLevel { get; private set; }
        [field: SerializeField] public TowerConfig FourthLevelAlt { get; private set; }
    }
}
