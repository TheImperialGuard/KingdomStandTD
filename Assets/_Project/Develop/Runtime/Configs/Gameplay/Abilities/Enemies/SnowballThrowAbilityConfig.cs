using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies
{
    // Бросок снежка: по событию клипа каста враг бросает снежок в случайную башню в радиусе, и та не стреляет заданное время.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/Enemies/SnowballThrowAbilityConfig", fileName = "SnowballThrowAbilityConfig")]
    public class SnowballThrowAbilityConfig : EnemyAbilityConfig
    {
        // Радиус выбора башни от бросающего, по земле.
        [field: SerializeField, Min(0)] public float ThrowRange { get; private set; } = 8;
        [field: SerializeField, Min(0)] public float FreezeDuration { get; private set; } = 4;

        [field: SerializeField] public string SnowballPrefabPath { get; private set; } = "Prefabs/Entities/Projectiles/Snowball";

        // Скорость по земле: время полёта растёт с расстоянием. Высота дуги — над прямой от руки до цели.
        [field: SerializeField, Min(0.01f)] public float SnowballSpeed { get; private set; } = 6;
        [field: SerializeField, Min(0)] public float SnowballArcHeight { get; private set; } = 1.5f;
    }
}
