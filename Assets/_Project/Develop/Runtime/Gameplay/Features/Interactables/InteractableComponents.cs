using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class IsInteractable : IEntityComponent
    {
    }

    public class CanInteract : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class InteractRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class InteractEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class InteractiveAction : IEntityComponent
    {
        public ReactiveVariable<IInteractAction> Value;
    }
}
