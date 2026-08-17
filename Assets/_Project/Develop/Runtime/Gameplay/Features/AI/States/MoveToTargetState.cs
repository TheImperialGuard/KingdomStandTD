using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class MoveToTargetState : State, IUpdatableState
    {
        private ReactiveVariable<Entity> _currentTarget;

        private ReactiveVariable<Vector3> _movementDirection;

        private Transform _transform;

        public MoveToTargetState(Entity entity)
        {
            _currentTarget = entity.CurrentTarget;
            _movementDirection = entity.MoveDirection;
            _transform = entity.Transform;

        }

        public void Update(float deltaTime)
        {
            if (_currentTarget.Value != null && _currentTarget.Value.Transform != null)
                _movementDirection.Value = (_currentTarget.Value.Transform.position - _transform.position).normalized;
        }
    }
}
