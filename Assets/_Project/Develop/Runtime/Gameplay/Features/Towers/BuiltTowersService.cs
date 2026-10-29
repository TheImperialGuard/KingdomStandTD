using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    // Построенные башни уровня — сущности с типом башни (у подбашен и демо-башен его нет).
    // Улучшение создаёт новую сущность и освобождает старую, поэтому список держится на событиях EntitiesLifeContext.
    public class BuiltTowersService : IDisposable
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private readonly List<Entity> _towers = new();

        public BuiltTowersService(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;

            foreach (Entity entity in _entitiesLifeContext.Entities)
                OnEntityAdded(entity);

            _entitiesLifeContext.Added += OnEntityAdded;
            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
            _entitiesLifeContext.Released -= OnEntityReleased;

            _towers.Clear();
        }

        public bool HasTowerInRange(Vector3 position, float range)
        {
            foreach (Entity tower in _towers)
            {
                if (IsInRange(tower, position, range))
                    return true;
            }

            return false;
        }

        public void GetTowersInRange(Vector3 position, float range, List<Entity> result)
        {
            result.Clear();

            foreach (Entity tower in _towers)
            {
                if (IsInRange(tower, position, range))
                    result.Add(tower);
            }
        }

        // Расстояние — по земле: высота башни и точки, от которой меряют, не важна.
        private bool IsInRange(Entity tower, Vector3 position, float range)
        {
            if (tower.SelfReleaseRequested.Value)
                return false;

            Vector3 offset = tower.Transform.position - position;
            offset.y = 0;

            return offset.sqrMagnitude <= range * range;
        }

        private void OnEntityAdded(Entity entity)
        {
            if (entity.HasComponent<TowerType>())
                _towers.Add(entity);
        }

        private void OnEntityReleased(Entity entity) => _towers.Remove(entity);
    }
}
