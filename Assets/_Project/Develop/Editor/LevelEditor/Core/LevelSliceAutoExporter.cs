using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Обновляет срезы, когда сохраняются, перемещаются или удаляются конфиги баланса и префабы уровней.
    // Экспорт откладывается до конца импорта и выполняется один раз на пачку изменений.
    public class LevelSliceAutoExporter : AssetPostprocessor
    {
        private const string ASSET_EXTENSION = ".asset";
        private const string PREFAB_EXTENSION = ".prefab";
        private const string RESOURCES_FOLDER = "/Resources/";
        private const string CHANGES_SEPARATOR = ", ";

        private static readonly Type[] _balanceConfigTypes =
        {
            typeof(LevelConfig),
            typeof(LevelsListConfig),
            typeof(GameConfig),
            typeof(CharacterConfig),
            typeof(ShootingTowerConfig),
            typeof(TowerUpgradeTreeConfig),
            typeof(TowersListConfig),
        };

        private static bool _isExportScheduled;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (_isExportScheduled)
                return;

            if (HasChangedBalanceAssets(importedAssets, movedAssets) == false
                && HasRemovedBalanceAssets(deletedAssets, movedFromAssetPaths) == false)
            {
                return;
            }

            _isExportScheduled = true;
            EditorApplication.delayCall += ExportScheduled;
        }

        private static void ExportScheduled()
        {
            _isExportScheduled = false;

            List<string> changes = LevelSliceExporter.ExportAll();

            if (changes.Count == 0)
                return;

            string changesText = string.Join(CHANGES_SEPARATOR, changes);
            Debug.Log($"Срезы баланса: {changesText}");
        }

        private static bool HasChangedBalanceAssets(string[] importedAssets, string[] movedAssets)
        {
            List<string> levelPrefabSuffixes = null;

            foreach (string path in importedAssets.Concat(movedAssets))
            {
                if (path.EndsWith(ASSET_EXTENSION, StringComparison.OrdinalIgnoreCase) && IsBalanceConfig(path))
                    return true;

                if (path.EndsWith(PREFAB_EXTENSION, StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                levelPrefabSuffixes ??= GetLevelPrefabSuffixes();

                if (levelPrefabSuffixes.Any(suffix => path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            return false;
        }

        // Тип удалённого ассета уже не узнать, поэтому подходит любой .asset или .prefab: конфиг врага,
        // на который ссылается уровень, может лежать где угодно. Лишний экспорт файлы не перезапишет.
        private static bool HasRemovedBalanceAssets(string[] deletedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in deletedAssets.Concat(movedFromAssetPaths))
            {
                if (path.EndsWith(ASSET_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(PREFAB_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsBalanceConfig(string path)
        {
            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

            if (assetType == null)
                return false;

            return _balanceConfigTypes.Any(configType => configType.IsAssignableFrom(assetType));
        }

        // Префаб уровня грузится через Resources по LevelConfig.PrefabPath — его путь заканчивается так.
        private static List<string> GetLevelPrefabSuffixes()
        {
            return LevelEditorAssets.FindLevelConfigs()
                .Where(level => string.IsNullOrWhiteSpace(level.PrefabPath) == false)
                .Select(level => RESOURCES_FOLDER + level.PrefabPath + PREFAB_EXTENSION)
                .ToList();
        }
    }
}
