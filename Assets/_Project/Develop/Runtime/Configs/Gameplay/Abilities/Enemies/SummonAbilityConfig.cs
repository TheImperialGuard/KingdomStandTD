using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies
{
    // Призыв: по событию клипа каста вокруг призывающего поднимается строй врагов, и они идут по его маршруту.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/Enemies/SummonAbilityConfig", fileName = "SummonAbilityConfig")]
    public class SummonAbilityConfig : EnemyAbilityConfig
    {
        [field: SerializeField] public MeleeConfig SummonedConfig { get; private set; }
        [field: SerializeField, Min(0)] public int Count { get; private set; } = 4;

        // Шаг строя вдоль дороги и поперёк неё.
        [field: SerializeField, Min(0.01f)] public float RowSpacing { get; private set; } = 0.5f;
        [field: SerializeField, Min(0.01f)] public float ColumnSpacing { get; private set; } = 0.5f;
    }
}
