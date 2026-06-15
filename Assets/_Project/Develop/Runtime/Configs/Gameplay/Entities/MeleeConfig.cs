using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/NewMeleeConfig", fileName = "MeleeConfig")]
    public class MeleeConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Prefabs/Entities/Melee";
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)] public float RotateSpeed { get; private set; } = 900;
    }
}
