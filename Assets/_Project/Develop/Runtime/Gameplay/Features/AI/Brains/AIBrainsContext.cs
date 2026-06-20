using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains
{
    public class AIBrainsContext : IDisposable
    {
        private readonly List<EntityToBrain> _entityToBrains = new List<EntityToBrain>();

        public void SetBrainsFor(Entity entity, IBrain brain)
        {
            foreach (EntityToBrain item in _entityToBrains)
            {
                if (item.Entity == entity)
                {
                    item.Brain.Disable();
                    item.Brain.Dispose();

                    item.Brain = brain;
                    item.Brain.Enable();

                    return;
                }
            }

            EntityToBrain entityToBrain = new EntityToBrain(entity, brain);

            _entityToBrains.Add(entityToBrain);

            brain.Enable();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _entityToBrains.Count; i++)
            {
                if (_entityToBrains[i].Entity.IsInit == false)
                {
                    int lastIndex = _entityToBrains.Count - 1;

                    _entityToBrains[i].Brain.Dispose();
                    _entityToBrains[i] = _entityToBrains[lastIndex];
                    _entityToBrains.RemoveAt(lastIndex);
                    i--;
                    continue;
                }

                _entityToBrains[i].Brain.Update(deltaTime);
            }
        }

        public void Dispose()
        {
            foreach (EntityToBrain item in _entityToBrains)
                item.Brain.Dispose();

            _entityToBrains.Clear();
        }

        private class EntityToBrain
        {
            public Entity Entity;
            public IBrain Brain;

            public EntityToBrain(Entity entity, IBrain brain)
            {
                Entity = entity;
                Brain = brain;
            }
        }
    }
}
