using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SummonFeature
{
    public class SummonAnimationEventReceiverComponent : IEntityComponent
    {
        public SummonAnimationEventReceiver Value;
    }

    public class SummonedEnemyConfig : IEntityComponent
    {
        public MeleeConfig Value;
    }

    public class SummonCount : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class SummonRowSpacing : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SummonColumnSpacing : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
