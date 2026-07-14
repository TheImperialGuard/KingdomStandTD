using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateTowerPlaceholder(Vector3 position, TowerPlaceholderConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
