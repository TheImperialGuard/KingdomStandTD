using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class AreaEffectRadius : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class MustStunOnAttack : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class MustCreateFragmentsOnProjectileDeath : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class FragmentsDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class FragmentsCount : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
