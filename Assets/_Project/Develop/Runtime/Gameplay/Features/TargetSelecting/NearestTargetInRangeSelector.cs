using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public class NearestTargetInRangeSelector : ITargetSelector
    {
        private Entity _source;

        private Transform _sourceTransform;

        public NearestTargetInRangeSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Transform;
        }
        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            IEnumerable<Entity> selectedTargets = targets.Where(target =>
            {
                bool result = target != _source;

                float distanceToTarget = GetDistanceTo(target);

                result = result && (distanceToTarget <= _source.InstantShootRange.Value);

                result = result && (target.TryGetComponent<IsProjectile>(out IsProjectile isProjectile) == false);

                return result;
            });

            if (selectedTargets.Any() == false)
                return null;

            Entity closestTarget = selectedTargets.First();

            float minDistance = GetDistanceTo(closestTarget);

            foreach (Entity target in selectedTargets)
            {
                float distance = GetDistanceTo(target);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        private float GetDistanceTo(Entity target) => (_sourceTransform.position - target.Transform.position).magnitude;
    }
}
