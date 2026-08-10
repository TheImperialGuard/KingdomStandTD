using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using System.Linq;
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

        public abstract Entity SelectTargetFrom(IEnumerable<Entity> targets);

        public abstract List<Entity> SelectMultipleTargetsFrom(IEnumerable<Entity> targets, int count);

        protected Entity GetClosestTargetFrom(IEnumerable<Entity> targets)
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

        protected bool TryExcludeTargetsFrom(IEnumerable<Entity> targets, List<ReactiveVariable<Entity>> targetsForExclude, out IEnumerable<Entity> filteredTargets)
        {
            filteredTargets = ExcludeTargetsFrom(targets, targetsForExclude);

            return filteredTargets.Any();
        }

        protected IEnumerable<Entity> ExcludeTargetsFrom(IEnumerable<Entity> targets, List<ReactiveVariable<Entity>> targetsForExclude)
        {
            List<Entity> targetsForExcludeValues = new();

            foreach(ReactiveVariable<Entity> target in targetsForExclude)
                targetsForExcludeValues.Add(target.Value);

            return targets.Where(target => targetsForExcludeValues.Contains(target) == false);
        }

        protected bool TryGetDamagableTargets(IEnumerable<Entity> targets, out IEnumerable<Entity> damagables)
        {
            damagables = GetDamagableTargetsFrom(targets);

            return damagables.Any();
        }

        protected IEnumerable<Entity> GetDamagableTargetsFrom(IEnumerable<Entity> targets)
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

        protected bool TryGetTargetsInRange(IEnumerable<Entity> targets, out IEnumerable<Entity> targetsInRange)
        {
            targetsInRange = GetTargetsInRangeFrom(targets);

            return targetsInRange.Any();
        }

        protected IEnumerable<Entity> GetTargetsInRangeFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => GetDistanceTo(target) <= _source.InstantShootRange.Value);
        }

        protected bool TryGetEnemies(IEnumerable<Entity> targets, out IEnumerable<Entity> enemies)
        {
            enemies = GetEnemiesFrom(targets);

            return enemies.Any();
        }

        protected IEnumerable<Entity> GetEnemiesFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => EntitiesHelper.IsSameTeam(_source, target) == false);
        }

        protected bool TryGetTeamMember(IEnumerable<Entity> targets, out IEnumerable<Entity> teamMembers)
        {
            teamMembers = GetTeamMemberFrom(targets);

            return teamMembers.Any();
        }

        protected IEnumerable<Entity> GetTeamMemberFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => target.HasComponent<Team>());
        }

        protected float GetDistanceTo(Entity target) => (_sourceTransform.position - target.Transform.position).magnitude;
    }
}
