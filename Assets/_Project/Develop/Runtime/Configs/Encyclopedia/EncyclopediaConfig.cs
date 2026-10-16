using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Encyclopedia
{
    // Состав и порядок книги: записи разделов идут в порядке списков. Из дерева башни в книгу попадают уровни I–IV,
    // альтернативный четвёртый уровень не берётся.
    [CreateAssetMenu(menuName = "Configs/Encyclopedia/EncyclopediaConfig", fileName = "EncyclopediaConfig")]
    public class EncyclopediaConfig : ScriptableObject
    {
        [SerializeField] private List<EncyclopediaNoteConfig> _notes;
        [SerializeField] private List<TowerUpgradeTreeConfig> _towerTrees;
        [SerializeField] private List<MeleeConfig> _enemies;
        [SerializeField] private List<EncyclopediaStatInfo> _stats;

        [field: SerializeField] public EncyclopediaTraitInfo BossTrait { get; private set; }
        [field: SerializeField] public EncyclopediaTraitInfo FlyingTrait { get; private set; }

        public IReadOnlyList<EncyclopediaNoteConfig> Notes => _notes;
        public IReadOnlyList<TowerUpgradeTreeConfig> TowerTrees => _towerTrees;
        public IReadOnlyList<MeleeConfig> Enemies => _enemies;

        public EncyclopediaStatInfo GetStatInfo(EncyclopediaStats stat)
        {
            foreach (EncyclopediaStatInfo statInfo in _stats)
            {
                if (statInfo.Stat == stat)
                    return statInfo;
            }

            throw new ArgumentException($"Not found encyclopedia info for stat: {stat}");
        }
    }
}
