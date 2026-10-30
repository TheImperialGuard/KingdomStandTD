using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Ключ таблицы значений по волнам: на волне Wave значение равно Value.
    [Serializable]
    public class WaveValueKey
    {
        [field: SerializeField, Min(1)] public int Wave { get; private set; } = 1;
        [field: SerializeField] public float Value { get; private set; }
    }
}
