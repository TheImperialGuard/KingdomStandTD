using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage
{
    public class LastingDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LastingDamageInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LastingDamageInterval : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
