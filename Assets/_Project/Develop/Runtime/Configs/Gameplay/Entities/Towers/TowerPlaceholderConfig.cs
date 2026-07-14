using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewTowerPlaceholderConfig", fileName = "TowerPlaceholderConfig")]
    public class TowerPlaceholderConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
    }
}
