using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class TowersPlaceholdersService
    {
        private readonly Level _level;
        private readonly EntitiesFactory _entitiesFactory;
        private readonly TowerPlaceholderConfig _placeholderConfig;

        private List<Entity> _placeholders = new();

        public TowersPlaceholdersService(
            Level level,
            EntitiesFactory entitiesFactory,
            TowerPlaceholderConfig placeholderConfig)
        {
            _level = level;
            _entitiesFactory = entitiesFactory;
            _placeholderConfig = placeholderConfig;
        }

        public void CreateAllPlaceholders()
        {
            foreach (Transform transform in _level.TowersPositions)
                CreatePlaceholder(transform.position);
        }

        public void CreatePlaceholder(Vector3 position)
        {
            Entity placeholder = _entitiesFactory.CreateTowerPlaceholder(position, _placeholderConfig);

            _placeholders.Add(placeholder);
        }

        public void ReleasePlaceholder(Entity placeholder)
        {
            placeholder.SelfReleaseRequested.Value = true;
            _placeholders.Remove(placeholder);
        }
    }
}
