using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Towers
{

    [CreateAssetMenu(menuName = "Configs/Gameplay/Towers/NewTowerConfig", fileName = "TowerConfig")]
    public class TowerConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; }

        [field: SerializeField, Min(0)] public float AttackRange { get; private set; } = 3f;

        [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; } = 1f;

        [field: SerializeField, Min(0)] public float AttackDamage { get; private set; } = 2f;
    }
}
