using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    // Разворот к цели только по горизонтали. RotateToTargetState не годится: RigidbodyRotationSystem берёт LookRotation
    // от полного вектора, и модель наклонилась бы к цели выше или ниже себя.
    public class RotateToTargetOnGroundState : State, IUpdatableState
    {
        private readonly ReactiveVariable<Entity> _target;

        private readonly ReactiveVariable<Vector3> _rotationDirection;

        private readonly Transform _transform;

        public RotateToTargetOnGroundState(Entity entity, ReactiveVariable<Entity> target)
        {
            _target = target;
            _rotationDirection = entity.RotationDirection;
            _transform = entity.Transform;
        }

        public void Update(float deltaTime)
        {
            Entity target = _target.Value;

            // Освобождённая цель: её объект из пула уже может принадлежать другой сущности
            if (target == null || target.IsInit == false)
                return;

            Vector3 direction = Vector3.ProjectOnPlane(target.Transform.position - _transform.position, Vector3.up);

            if (direction == Vector3.zero)
                return;

            _rotationDirection.Value = direction.normalized;
        }
    }
}
