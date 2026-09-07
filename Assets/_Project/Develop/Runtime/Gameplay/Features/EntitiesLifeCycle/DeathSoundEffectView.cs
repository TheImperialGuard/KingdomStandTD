using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class DeathSoundEffectView : EntityView
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField, Range(-3f, 3f)] private float _pitchUpperLimit = 1f;
        [SerializeField, Range(-3f, 3f)] private float _pitchLowerLimit = 1f;

        private IReadOnlyVariable<bool> _isDead;

        private IDisposable _isDeadChangedDisposable;

        private void OnValidate()
        {
            if (_pitchLowerLimit > _pitchUpperLimit)
                _pitchUpperLimit = _pitchLowerLimit;
        }

        public override void Cleanup(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            base.Cleanup(entity);

            _isDeadChangedDisposable.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            _isDead = entity.IsDead;

            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead == false)
                return;

            _audioSource.pitch = Random.Range(_pitchLowerLimit, _pitchUpperLimit);
            _audioSource.PlayOneShot(_audioClip);
        }
    }
}
