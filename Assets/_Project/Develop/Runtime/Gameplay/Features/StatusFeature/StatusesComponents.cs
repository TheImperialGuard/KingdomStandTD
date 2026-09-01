using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class Statuses : IEntityComponent
    {
        public StatusesList Value;
    }

    public class IsStunned : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class StunDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
