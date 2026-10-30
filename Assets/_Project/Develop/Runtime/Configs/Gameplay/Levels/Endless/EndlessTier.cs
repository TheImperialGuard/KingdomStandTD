using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Тир врагов бесконечного режима: его доля в числе врагов волны и враги, из которых генератор выбирает виды на волну.
    // Имя стоит первым полем — по нему Unity подписывает элементы списка тиров.
    [Serializable]
    public class EndlessTier
    {
        [SerializeField] private string _name;
        [SerializeField] private List<EndlessTierEnemy> _enemies = new();

        [field: Tooltip("Доля тира в числе врагов волны. Доли тиров не обязаны давать в сумме 1: делятся пропорционально")]
        [field: SerializeField] public WaveValueTable Share { get; private set; } = new();

        [field: Tooltip("Сколько разных видов тира бывает в одной волне")]
        [field: SerializeField, Min(1)] public int KindsPerWave { get; private set; } = 2;

        public string Name => _name;
        public IReadOnlyList<EndlessTierEnemy> Enemies => _enemies;
    }
}
