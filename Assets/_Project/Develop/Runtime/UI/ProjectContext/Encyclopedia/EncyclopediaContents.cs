using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Encyclopedia;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Записи разделов книги в порядке EncyclopediaConfig: из дерева башни — уровни I–IV, альтернативный четвёртый не берётся.
    public class EncyclopediaContents
    {
        private readonly EncyclopediaConfig _config;
        private readonly List<ShootingTowerConfig> _towers = new();
        private readonly HashSet<ShootingTowerConfig> _topLevelTowers = new();

        public EncyclopediaContents(EncyclopediaConfig config)
        {
            _config = config;

            foreach (TowerUpgradeTreeConfig tree in config.TowerTrees)
            {
                _towers.Add(tree.FirstLevel);
                _towers.Add(tree.SecondLevel);
                _towers.Add(tree.ThirdLevel);
                _towers.Add(tree.FourthLevel);
                _topLevelTowers.Add(tree.FourthLevel);
            }
        }

        public IReadOnlyList<EncyclopediaNoteConfig> Notes => _config.Notes;

        public IReadOnlyList<ShootingTowerConfig> Towers => _towers;

        public IReadOnlyList<MeleeConfig> Enemies => _config.Enemies;

        public int GetRecordsCount(EncyclopediaSections section)
        {
            switch (section)
            {
                case EncyclopediaSections.Tutorial:
                    return Notes.Count;

                case EncyclopediaSections.Towers:
                    return Towers.Count;

                case EncyclopediaSections.Enemies:
                    return Enemies.Count;

                default:
                    throw new ArgumentOutOfRangeException(nameof(section), section, null);
            }
        }

        // Способности в книге есть только у башен IV уровня.
        public bool IsTopLevel(ShootingTowerConfig tower) => _topLevelTowers.Contains(tower);

        public EncyclopediaPage FindPage(EncyclopediaNoteConfig note)
            => FindPage(EncyclopediaSections.Tutorial, Notes, note);

        public EncyclopediaPage FindPage(ShootingTowerConfig tower)
            => FindPage(EncyclopediaSections.Towers, Towers, tower);

        public EncyclopediaPage FindPage(CharacterConfig enemy)
            => FindPage(EncyclopediaSections.Enemies, Enemies, enemy as MeleeConfig);

        // Записи нет в книге — первый разворот её раздела (Р-14).
        private EncyclopediaPage FindPage<TRecord>(
            EncyclopediaSections section,
            IReadOnlyList<TRecord> records,
            TRecord record) where TRecord : ScriptableObject
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i] == record)
                    return new EncyclopediaPage(section, i);
            }

            Debug.LogWarning($"Encyclopedia has no record for {record}, opening the first spread of {section}");

            return new EncyclopediaPage(section, 0);
        }
    }
}
