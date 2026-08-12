using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class CurrentTarget : IEntityComponent
    {
        public ReactiveVariable<Entity> Value;
    }

    public class MaxTargets : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class TargetsForExclude : IEntityComponent
    {
        public List<ReactiveVariable<Entity>> Value;
    }
}
