using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Encyclopedia
{
    // Иконка и подпись плитки характеристики.
    [Serializable]
    public class EncyclopediaStatInfo
    {
        [field: SerializeField] public EncyclopediaStats Stat { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Caption { get; private set; }
    }
}
