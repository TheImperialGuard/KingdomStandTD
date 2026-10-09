using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    // Растягивает анимацию подъёма на время подъёма из конфига через Speed Multiplier её состояния.
    [RequireComponent(typeof(Animator))]
    public class SpawnProcessAnimationView : EntityView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _spawnClip;

        [SerializeField] private string _spawnSpeedKeyName;

        private int SpawnSpeedKey => Animator.StringToHash(_spawnSpeedKeyName);

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetInSpawnProcess(out _) == false)
                return;

            _animator.SetFloat(SpawnSpeedKey, _spawnClip.length / entity.SpawnProcessTimerC.InitialTime.Value);
        }

        private void OnValidate()
        {
            _animator = _animator != null ? _animator : GetComponent<Animator>();
        }
    }
}
