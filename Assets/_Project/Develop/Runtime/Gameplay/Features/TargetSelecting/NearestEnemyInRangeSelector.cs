using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public class NearestEnemyInRangeSelector : ITargetSelector
    {
        private Entity _source;

        private Transform _sourceTransform;

        public NearestEnemyInRangeSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Transform;
        }
        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            IEnumerable<Entity> selectedTargets = GetEnemiesFrom(targets);

            selectedTargets = GetTargetsInRangeFrom(selectedTargets);

            selectedTargets = GetDamagableTargetsFrom(selectedTargets);

            if (selectedTargets.Any() == false)
                return null;

            Entity closestTarget = GetClosestTargetFrom(selectedTargets);

            return closestTarget;
        }

        private Entity GetClosestTargetFrom(IEnumerable<Entity> targets)
        {
            Entity closestTarget = targets.First();

            float minDistance = GetDistanceTo(closestTarget);

            foreach (Entity target in targets)
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

        private IEnumerable<Entity> GetDamagableTargetsFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target =>
            {
                bool result = target.HasComponent<TakeDamageRequest>();

                if (target.TryGetCanApplyDamage(out ICompositeCondition canApplyDamage))
                {
                    result = result && canApplyDamage.Evaluate();
                }

                return result;
            });
        }

        private IEnumerable<Entity> GetTargetsInRangeFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => GetDistanceTo(target) <= _source.InstantShootRange.Value);
        }

        private IEnumerable<Entity> GetEnemiesFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => EntitiesHelper.IsSameTeam(_source, target) == false);
        }

        private float GetDistanceTo(Entity target) => (_sourceTransform.position - target.Transform.position).magnitude;
    }
}
