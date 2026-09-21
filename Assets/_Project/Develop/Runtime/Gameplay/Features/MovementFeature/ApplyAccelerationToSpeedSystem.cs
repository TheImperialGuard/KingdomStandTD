using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class ApplyAccelerationToSpeedSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<float> _speedAcceleration;
        private ReactiveVariable<float> _maxMoveSpeed;
        private ReactiveVariable<bool> _isMoving;

        public void OnInit(Entity entity)
        {
            _moveSpeed = entity.MoveSpeed;
            _isMoving = entity.IsMoving;
            _speedAcceleration = entity.SpeedAcceleration;
            _maxMoveSpeed = entity.MaxMoveSpeed;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isMoving.Value == false)
                return;

            if (_moveSpeed.Value >= _maxMoveSpeed.Value)
                return;

            float boost = _speedAcceleration.Value * deltaTime;
            _moveSpeed.Value = Mathf.Min(_moveSpeed.Value + boost, _maxMoveSpeed.Value);
        }
    }
}
