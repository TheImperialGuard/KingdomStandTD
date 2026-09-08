using Assets._Project.Develop.Runtime.Configs;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.ConfigsManagment
{
    public class ResourcesConfigsLoader : IConfigsLoader
    {
        private readonly ResourcesAssetsLoader _resources;

        private readonly Dictionary<Type, string> _configsResourcesPaths = new()
        {
            {typeof(LevelsListConfig), "Configs/Gameplay/Levels/LevelsListConfig" },
            {typeof(TowerPlaceholderConfig), "Configs/Gameplay/Entities/Towers/TowerPlaceholderConfig" },
            {typeof(TowersListConfig), "Configs/Gameplay/Entities/Towers/TowersListConfig" },
            {typeof(GameConfig), "Configs/Gameplay/GameConfig" },
            {typeof(ProjectilesListConfig), "Configs/Gameplay/Entities/Projectiles/ProjectilesListConfig" },
            {typeof(TowersAbilitiesListConfig), "Configs/Gameplay/Entities/Towers/TowersAbilitiesListConfig" },
            {typeof(MusicConfig), "Configs/MusicConfig" },
        };

        public ResourcesConfigsLoader(ResourcesAssetsLoader resources)
        {
            _resources = resources;
        }

        public IEnumerator LoadAsync(Action<Dictionary<Type, object>> onConfigsLoaded)
        {
            Dictionary<Type, object> loadedConfigs = new();

            foreach (KeyValuePair<Type, string> configResourcesPath in _configsResourcesPaths)
            {
                ScriptableObject config = _resources.Load<ScriptableObject>(configResourcesPath.Value);
                loadedConfigs.Add(configResourcesPath.Key, config);
                yield return null;
            }

            onConfigsLoaded?.Invoke(loadedConfigs);
        }
    }
}