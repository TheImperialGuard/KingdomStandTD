using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters
{
    public abstract class CharacterConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public string CharacterName { get; private set; }

        // Картинка записи в энциклопедии; пока её нет, книга показывает заглушку.
        [field: SerializeField] public Sprite Picture { get; private set; }
    }
}
