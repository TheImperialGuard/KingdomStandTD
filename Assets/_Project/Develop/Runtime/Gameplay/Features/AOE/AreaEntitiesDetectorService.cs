using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class AreaEntitiesDetectorService
    {
        private readonly CollidersRegistryService _collidersRegistryService;

        public const int MaxColliders = 64;
        private Collider[] _hitColliders = new Collider[MaxColliders];

        public AreaEntitiesDetectorService(CollidersRegistryService collidersRegistryService)
        {
            _collidersRegistryService = collidersRegistryService;
        }

        public List<Entity> GetEntitiesInArea(Vector3 center, float radius, int maxCount = 0)
        {
            List<Entity> entities = new List<Entity>();

            int numColliders = Physics.OverlapSphereNonAlloc(center, radius, _hitColliders);

            for (int i = 0; i < numColliders; i++)
            {
                Entity contactEntity = _collidersRegistryService.GetBy(_hitColliders[i]);

                if (contactEntity != null)
                {
                    entities.Add(contactEntity);
                    if (maxCount > 0 && entities.Count == maxCount)
                        break;
                }
            }

            return entities;
        }
    }
}
