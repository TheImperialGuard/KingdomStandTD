using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Поиск конфигов, с которыми работает редактор уровней.
    public static class LevelEditorAssets
    {
        // Сначала уровни кампании в порядке LevelsListConfig, затем остальные LevelConfig по имени.
        public static List<LevelConfig> FindLevelConfigs()
        {
            List<LevelConfig> levels = new();
            LevelsListConfig campaign = FindLevelsList();

            if (campaign != null && campaign.Levels != null)
            {
                foreach (LevelConfig level in campaign.Levels)
                {
                    if (level != null && levels.Contains(level) == false)
                        levels.Add(level);
                }
            }

            IEnumerable<LevelConfig> otherLevels = FindAll<LevelConfig>()
                .Where(level => levels.Contains(level) == false)
                .OrderBy(level => level.name, StringComparer.Ordinal);

            levels.AddRange(otherLevels.ToList());

            return levels;
        }

        public static LevelConfig FindLevelConfig(string levelName)
            => FindAll<LevelConfig>().FirstOrDefault(level => level.name == levelName);

        public static int? GetCampaignNumber(LevelsListConfig campaign, LevelConfig level)
        {
            if (campaign == null || campaign.Levels == null)
                return null;

            for (int i = 0; i < campaign.Levels.Count; i++)
            {
                if (campaign.Levels[i] == level)
                    return i + 1;
            }

            return null;
        }

        public static LevelsListConfig FindLevelsList() => FindAll<LevelsListConfig>().FirstOrDefault();

        public static GameConfig FindGameConfig() => FindAll<GameConfig>().FirstOrDefault();

        public static TowersListConfig FindTowersList() => FindAll<TowersListConfig>().FirstOrDefault();

        public static List<CharacterConfig> FindEnemyConfigs()
            => FindAll<CharacterConfig>().OrderBy(enemy => enemy.name, StringComparer.Ordinal).ToList();

        public static List<ShootingTowerConfig> FindTowerConfigs()
            => FindAll<ShootingTowerConfig>().OrderBy(tower => tower.name, StringComparer.Ordinal).ToList();

        private static List<T> FindAll<T>() where T : Object
        {
            List<T> assets = new();
            string typeFilter = "t:" + typeof(T).Name;

            foreach (string guid in AssetDatabase.FindAssets(typeFilter))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                    assets.Add(asset);
            }

            return assets;
        }
    }
}
