using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class RotateToTargetState : State, IUpdatableState
    {
        private ReactiveVariable<Entity> _currentTarget;

        private ReactiveVariable<Vector3> _rotationDirection;

        private Transform _transform;

        public RotateToTargetState(Entity entity)
        {
            _currentTarget = entity.CurrentTarget;
            _rotationDirection = entity.RotationDirection;
            _transform = entity.Transform;

        }

        public void Update(float deltaTime)
        {
            if (_currentTarget != null)
                _rotationDirection.Value = (_currentTarget.Value.Transform.position - _transform.position).normalized;
        }
    }
}
