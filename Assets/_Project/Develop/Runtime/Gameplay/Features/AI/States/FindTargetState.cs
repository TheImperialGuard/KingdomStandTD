using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class FindTargetState : State, IUpdatableState
    {
        private ITargetSelector _targetSelector;

        private TargetCandidatesService _targetCandidatesService;

        private ReactiveVariable<Entity> _currentTarget;

        public FindTargetState(
            ITargetSelector targetSelector,
            TargetCandidatesService targetCandidatesService,
            Entity entity)
        {
            _targetSelector = targetSelector;
            _targetCandidatesService = targetCandidatesService;
            _currentTarget = entity.CurrentTarget;
        }

        public void Update(float deltaTime)
        {
            _currentTarget.Value = _targetSelector.SelectTargetFrom(_targetCandidatesService.Candidates);
        }
    }
}
