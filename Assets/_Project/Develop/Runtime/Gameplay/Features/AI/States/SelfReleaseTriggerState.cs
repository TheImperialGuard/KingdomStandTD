using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class SelfReleaseTriggerState : State, IUpdatableState
    {
        private ReactiveVariable<bool> _selfReleaseRequested;

        public SelfReleaseTriggerState(Entity entity)
        {
            _selfReleaseRequested = entity.SelfReleaseRequested;
        }

        public override void Enter()
        {
            base.Enter();

            _selfReleaseRequested.Value = true;
        }

        public void Update(float deltaTime)
        {
        }
    }
}
