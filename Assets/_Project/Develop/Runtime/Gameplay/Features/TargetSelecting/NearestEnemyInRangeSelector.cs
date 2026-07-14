using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            if (TryGetTeamMember(targets, out IEnumerable<Entity> selectedTargets) == false)
                return null;

            if (TryGetEnemies(selectedTargets, out selectedTargets) == false)
                return null;

            if (TryGetTargetsInRange(selectedTargets, out selectedTargets) == false)
                return null;

            if (TryGetDamagableTargets(selectedTargets, out selectedTargets) == false)
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

        private bool TryGetDamagableTargets(IEnumerable<Entity> targets, out IEnumerable<Entity> damagables)
        {
            damagables = GetDamagableTargetsFrom(targets);

            return damagables.Any();
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

        private bool TryGetTargetsInRange(IEnumerable<Entity> targets, out IEnumerable<Entity> targetsInRange)
        {
            targetsInRange = GetTargetsInRangeFrom(targets);

            return targetsInRange.Any();
        }

        private IEnumerable<Entity> GetTargetsInRangeFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => GetDistanceTo(target) <= _source.InstantShootRange.Value);
        }

        private bool TryGetEnemies(IEnumerable<Entity> targets, out IEnumerable<Entity> enemies)
        {
            enemies = GetEnemiesFrom(targets);

            return enemies.Any();
        }

        private IEnumerable<Entity> GetEnemiesFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => EntitiesHelper.IsSameTeam(_source, target) == false);
        }

        private bool TryGetTeamMember(IEnumerable<Entity> targets, out IEnumerable<Entity> teamMembers)
        {
            teamMembers = GetTeamMemberFrom(targets);

            return teamMembers.Any();
        }

        private IEnumerable<Entity> GetTeamMemberFrom(IEnumerable<Entity> targets)
        {
            return targets.Where(target => target.HasComponent<Team>());
        }

        private float GetDistanceTo(Entity target) => (_sourceTransform.position - target.Transform.position).magnitude;
    }
}
