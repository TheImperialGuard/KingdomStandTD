using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters
{
    public abstract class CharacterConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public string CharacterName { get; private set; }
    }
}
