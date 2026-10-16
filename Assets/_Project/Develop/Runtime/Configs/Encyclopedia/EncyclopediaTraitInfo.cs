using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Encyclopedia
{
    // Признак врага на левой странице: иконка, название и пояснение, как он меняет бой.
    [Serializable]
    public class EncyclopediaTraitInfo
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(2, 4)] public string Description { get; private set; }
    }
}
