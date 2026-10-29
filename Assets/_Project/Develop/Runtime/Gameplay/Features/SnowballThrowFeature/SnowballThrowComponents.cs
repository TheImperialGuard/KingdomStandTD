using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature
{
    public class SnowballThrowAnimationEventReceiverComponent : IEntityComponent
    {
        public SnowballThrowAnimationEventReceiver Value;
    }

    public class SnowballThrowTarget : IEntityComponent
    {
        public ReactiveVariable<Entity> Value;
    }

    public class SnowballThrowRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SnowballSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SnowballArcHeight : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SnowballPrefabPath : IEntityComponent
    {
        public ReactiveVariable<string> Value;
    }

    public class FreezeTarget : IEntityComponent
    {
        public ReactiveVariable<Entity> Value;
    }
}
