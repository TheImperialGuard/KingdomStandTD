using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    [RequireComponent(typeof(Animator))]
    public class RunningView : EntityView
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private string _isRunningKeyName;

        private IReadOnlyVariable<bool> _isMoving;

        private IDisposable _isMovingChangedDisposable;

        private int IsRunningKey => Animator.StringToHash(_isRunningKeyName);

        private void OnValidate()
        {
            _animator = _animator != null ? _animator : GetComponent<Animator>();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isMovingChangedDisposable.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isMoving = entity.IsMoving;

            _isMovingChangedDisposable = _isMoving.Subscribe(OnIsMovingChanged);

            UpdateRunningView(_isMoving.Value);
        }

        private void OnIsMovingChanged(bool oldIsMoving, bool isMoving) => UpdateRunningView(isMoving);

        private void UpdateRunningView(bool value) => _animator.SetBool(IsRunningKey, value);
    }
}
