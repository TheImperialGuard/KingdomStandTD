using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies
{
    // Способность врага применяется сама по перезарядке: перезарядка стартует при появлении врага и после каждого каста.
    public abstract class EnemyAbilityConfig : ScriptableObject
    {
        // Название, описание и иконка — для энциклопедии. Чисел в описании нет: строку цифр книга собирает из полей.
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(2, 4)] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        // Длительность каста: враг всё это время стоит, анимация способности растягивается под неё.
        [field: SerializeField, Min(0.01f)] public float Duration { get; private set; } = 1;
        [field: SerializeField, Min(0)] public float Cooldown { get; private set; } = 10;
    }
}
