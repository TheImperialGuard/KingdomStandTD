using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class AbilitiesComponent : IEntityComponent
    {
        public AbilitiesList Value;
    }

    public class FirstAbilityCooldownTimer : IEntityComponent
    {
        public ReactiveVariable<float> InitialTime;
        public ReactiveVariable<float> ModifiedTime;
        public ReactiveVariable<float> CurrentTime;
    }

    public class SecondAbilityCooldownTimer : IEntityComponent
    {
        public ReactiveVariable<float> InitialTime;
        public ReactiveVariable<float> ModifiedTime;
        public ReactiveVariable<float> CurrentTime;
    }

    public class InFirstAbilityCooldown : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class InSecondAbilityCooldown : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class StartFirstAbilityRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class StartFirstAbilityEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class StartSecondAbilityRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class StartSecondAbilityEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class CanStartFirstAbility : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class CanStartSecondAbility : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class FirstAbilityProcessTimer : IEntityComponent
    {
        public ReactiveVariable<float> InitialTime;
        public ReactiveVariable<float> ModifiedTime;
        public ReactiveVariable<float> CurrentTime;
    }

    public class SecondAbilityProcessTimer : IEntityComponent
    {
        public ReactiveVariable<float> InitialTime;
        public ReactiveVariable<float> ModifiedTime;
        public ReactiveVariable<float> CurrentTime;
    }

    public class InFirstAbilityProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class InSecondAbilityProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class EndFirstAbilityEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class EndSecondAbilityEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class FirstAbilityDelayTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SecondAbilityDelayTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SecondAbilityDelayModifiedTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class FirstAbilityDelayModifiedTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SecondAbilityDelayEndEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class FirstAbilityDelayEndEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }
}
