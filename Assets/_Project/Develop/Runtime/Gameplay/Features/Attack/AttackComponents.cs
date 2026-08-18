using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackCooldownInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackCooldownModifiedTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackCooldownCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InAttackCooldown : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class MustCancelAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class AttackCancelEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class InstantAttackDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InstantAttackDamageType : IEntityComponent
    {
        public ReactiveVariable<DamageTypes> Value;
    }

    public class StartAttackRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class StartAttackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class CanStartAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class EndAttackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class AttackProcessInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackProcessModifiedTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackProcessCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InAttackProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class AttackDelayTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDelayModifiedTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDelayEndEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }
}
