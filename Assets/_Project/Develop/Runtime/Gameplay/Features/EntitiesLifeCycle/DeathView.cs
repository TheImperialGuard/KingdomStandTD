using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    [RequireComponent(typeof(Animator))]
    public class DeathView : EntityView
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private string _isDeadKeyKeyName;

        private IReadOnlyVariable<bool> _isDead;

        private IDisposable _isDeadChangedDisposable;

        private int IsDeadKey => Animator.StringToHash(_isDeadKeyKeyName);

        private void OnValidate()
        {
            _animator = _animator != null ? _animator : GetComponent<Animator>();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isDeadChangedDisposable.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDead = entity.IsDead;

            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);

            UpdateDeadView(_isDead.Value);
        }

        private void OnIsDeadChanged(bool oldIsMoving, bool isMoving) => UpdateDeadView(isMoving);

        private void UpdateDeadView(bool value) => _animator.SetBool(IsDeadKey, value);
    }
}
