using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SoundEffects
{
    public abstract class SoundEffectViewBase : EntityView
    {
        private const float MinPitch = 0.5f;
        private const float MaxPitch = 2f;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField, Range(MinPitch, MaxPitch)] private float _pitchUpperLimit = 1f;
        [SerializeField, Range(MinPitch, MaxPitch)] private float _pitchLowerLimit = 1f;

        public override void Cleanup(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            base.Cleanup(entity);

            Unsubscribe();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            Subscribe(entity);
        }

        protected abstract void Subscribe(Entity entity);

        protected abstract void Unsubscribe();

        protected void PlaySound()
        {
            if (SoundRetriggerLimiter.TryStart(_audioClip) == false)
                return;

            _audioSource.pitch = Random.Range(_pitchLowerLimit, _pitchUpperLimit);
            _audioSource.PlayOneShot(_audioClip);
        }

        private void OnValidate()
        {
            if (_pitchLowerLimit > _pitchUpperLimit)
                _pitchUpperLimit = _pitchLowerLimit;
        }
    }
}
