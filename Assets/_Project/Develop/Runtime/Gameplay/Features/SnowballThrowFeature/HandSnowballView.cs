using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature
{
    // Снежок в руке: появляется по событию клипа SpawnSnowball и пропадает, когда снежок брошен или бросающий умер.
    // В префабе объект выключен — иначе склейка частей модели вклеит его в меш врага.
    public class HandSnowballView : EntityView
    {
        [SerializeField] private GameObject _handSnowball;

        private SnowballThrowAnimationEventReceiver _receiver;
        private ReactiveVariable<bool> _isDead;

        private IDisposable _isDeadDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            if (_receiver != null)
            {
                _receiver.SnowballSpawned -= OnSnowballSpawned;
                _receiver.SnowballReleased -= OnSnowballReleased;
                _receiver = null;
            }

            _isDeadDisposable?.Dispose();
            _isDeadDisposable = null;

            // Объект из пула достанется новой сущности
            Hide();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetSnowballThrowAnimationEventReceiver(out _receiver) == false)
                return;

            _isDead = entity.IsDead;

            _receiver.SnowballSpawned += OnSnowballSpawned;
            _receiver.SnowballReleased += OnSnowballReleased;
            _isDeadDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        private void OnSnowballSpawned()
        {
            if (_isDead.Value == false)
                _handSnowball.SetActive(true);
        }

        private void OnSnowballReleased() => Hide();

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead)
                Hide();
        }

        private void Hide() => _handSnowball.SetActive(false);
    }
}
