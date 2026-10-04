using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class CoreBurstAnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _effect;

        [ContextMenu(nameof(CoreBurst))]
        public void CoreBurst()
        {
            _effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _effect.Play(true);
        }
    }
}
