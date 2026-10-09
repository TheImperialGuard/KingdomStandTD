using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies
{
    // Способность врага применяется сама по перезарядке: перезарядка стартует при появлении врага и после каждого каста.
    public abstract class EnemyAbilityConfig : ScriptableObject
    {
        // Длительность каста: враг всё это время стоит, анимация способности растягивается под неё.
        [field: SerializeField, Min(0.01f)] public float Duration { get; private set; } = 1;
        [field: SerializeField, Min(0)] public float Cooldown { get; private set; } = 10;
    }
}
