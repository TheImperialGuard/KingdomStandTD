using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public abstract class TargetSelector : ITargetSelector
    {
        protected readonly List<ReactiveVariable<Entity>> TargetsForExclude;

        private readonly Entity _source;

        private readonly Transform _sourceTransform;

        public TargetSelector(Entity entity, List<ReactiveVariable<Entity>> targetsForExclude = null)
        {
            TargetsForExclude = targetsForExclude;
            _source = entity;
            _sourceTransform = entity.Transform;
        }

        public abstract Entity SelectTargetFrom(IReadOnlyList<Entity> targets);

        public abstract List<Entity> SelectMultipleTargetsFrom(IReadOnlyList<Entity> targets, int count);

        protected bool IsTargetExcluded(Entity target)
        {
            if (TargetsForExclude == null)
                return false;

            for (int i = 0; i < TargetsForExclude.Count; i++)
            {
                if (TargetsForExclude[i].Value == target)
                    return true;
            }

            return false;
        }

        protected bool IsTeamMember(Entity target) => target.HasComponent<Team>();

        protected bool IsEnemy(Entity target) => EntitiesHelper.IsSameTeam(_source, target) == false;

        protected bool IsInShootRange(Entity target) => GetDistanceTo(target) <= _source.InstantShootRange.Value;

        protected bool IsDamagable(Entity target)
        {
            if (target.HasComponent<TakeDamageRequest>() == false)
                return false;

            if (target.TryGetCanApplyDamage(out ICompositeCondition canApplyDamage))
                return canApplyDamage.Evaluate();

            return true;
        }

        protected Entity GetClosestTargetFrom(IReadOnlyList<Entity> targets)
        {
            Entity closestTarget = targets[0];

            float minDistance = GetDistanceTo(closestTarget);

            for (int i = 1; i < targets.Count; i++)
            {
                float distance = GetDistanceTo(targets[i]);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = targets[i];
                }
            }

            return closestTarget;
        }

        protected Entity GetClosestToFinishTargetFrom(IReadOnlyList<Entity> targets)
        {
            Entity closestTargetToFinish = targets[0];

            float minDistanceToFinish = closestTargetToFinish.CurrentPathDistance.Value;

            for (int i = 1; i < targets.Count; i++)
            {
                float distance = targets[i].CurrentPathDistance.Value;

                if (distance < minDistanceToFinish)
                {
                    minDistanceToFinish = distance;
                    closestTargetToFinish = targets[i];
                }
            }

            return closestTargetToFinish;
        }

        protected float GetDistanceTo(Entity target) => (_sourceTransform.position - target.Transform.position).magnitude;
    }
}
