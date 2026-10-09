using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies
{
    // Боевой клич: пока вожак кричит, враги из списка целей в радиусе вокруг него получают ускорение.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Abilities/Enemies/WarCryAbilityConfig", fileName = "WarCryAbilityConfig")]
    public class WarCryAbilityConfig : EnemyAbilityConfig
    {
        [SerializeField] private List<EnemyKinds> _targetKinds = new() { EnemyKinds.Orc };

        [field: SerializeField, Min(0)] public float Radius { get; private set; } = 1.5f;
        [field: SerializeField, Min(1)] public float SpeedBoostIndex { get; private set; } = 1.5f;

        // Сколько ускорение держится после последней проверки зоны: выйдя из неё, цель остаётся быстрой ещё это время.
        [field: SerializeField, Min(0)] public float SpeedBoostDuration { get; private set; } = 3;

        public IReadOnlyList<EnemyKinds> TargetKinds => _targetKinds;
    }
}
