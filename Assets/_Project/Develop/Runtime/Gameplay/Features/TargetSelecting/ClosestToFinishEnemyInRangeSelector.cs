using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public class ClosestToFinishEnemyInRangeSelector : TargetSelector
    {
        private readonly List<Entity> _candidates = new(64);

        private readonly List<Entity> _multipleSelectionPool = new(64);

        private readonly List<Entity> _multipleSelectionResult = new(8);

        public ClosestToFinishEnemyInRangeSelector(
            Entity entity,
            List<ReactiveVariable<Entity>> targetsForExclude = null) : base(entity, targetsForExclude)
        {
        }

        public override Entity SelectTargetFrom(IReadOnlyList<Entity> targets)
        {
            _candidates.Clear();

            for (int i = 0; i < targets.Count; i++)
            {
                Entity target = targets[i];

                if (IsTargetExcluded(target))
                    continue;

                if (IsTeamMember(target) == false)
                    continue;

                if (IsEnemy(target) == false)
                    continue;

                if (IsInShootRange(target) == false)
                    continue;

                if (IsDamagable(target) == false)
                    continue;

                _candidates.Add(target);
            }

            if (_candidates.Count == 0)
                return null;

            return GetClosestToFinishTargetFrom(_candidates);
        }

        public override List<Entity> SelectMultipleTargetsFrom(IReadOnlyList<Entity> targets, int count)
        {
            _multipleSelectionPool.Clear();

            for (int i = 0; i < targets.Count; i++)
                _multipleSelectionPool.Add(targets[i]);

            _multipleSelectionResult.Clear();

            for (int i = 0; i < count; i++)
            {
                Entity target = SelectTargetFrom(_multipleSelectionPool);

                if (target == null)
                    break;

                _multipleSelectionResult.Add(target);
                _multipleSelectionPool.Remove(target);
            }

            return _multipleSelectionResult;
        }
    }
}
