using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public class ClosestToFinishEnemyInRangeSelector : TargetSelector
    {
        public ClosestToFinishEnemyInRangeSelector(
            Entity entity,
            List<ReactiveVariable<Entity>> targetsForExclude = null) : base(entity, targetsForExclude)
        {
        }

        public override Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            if (TargetsForExclude != null && TargetsForExclude.Any())
            {
                if (TryExcludeTargetsFrom(targets, TargetsForExclude, out targets) == false)
                    return null;
            }

            if (TryGetTeamMember(targets, out IEnumerable<Entity> selectedTargets) == false)
                return null;

            if (TryGetEnemies(selectedTargets, out selectedTargets) == false)
                return null;

            if (TryGetTargetsInRange(selectedTargets, out selectedTargets) == false)
                return null;

            if (TryGetDamagableTargets(selectedTargets, out selectedTargets) == false)
                return null;

            Entity closestTarget = GetClosestToFinishTargetFrom(selectedTargets);

            return closestTarget;
        }

        public override List<Entity> SelectMultipleTargetsFrom(IEnumerable<Entity> targets, int count)
        {
            List<Entity> targetsForSelecting = new List<Entity>(targets);

            List<Entity> selectedTargets = new();

            for (int i = 0; i < count; i++)
            {
                if (selectedTargets.Count > 0)
                {
                    foreach (Entity entity in selectedTargets)
                        targetsForSelecting.Remove(entity);
                }

                Entity target = SelectTargetFrom(targetsForSelecting);

                selectedTargets.Add(target);
            }

            return selectedTargets;
        }
    }
}
