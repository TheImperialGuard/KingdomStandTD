using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SummonFeature
{
    public class SummonAnimationEventReceiver : MonoBehaviour
    {
        public event Action Summoned;

        [SerializeField] private ParticleSystem[] _handGlowEffects;

        [ContextMenu(nameof(Charge))]
        public void Charge()
        {
            foreach (ParticleSystem effect in _handGlowEffects)
                effect.Play(true);
        }

        public void Summon() => Summoned?.Invoke();

        [ContextMenu(nameof(Release))]
        public void Release()
        {
            foreach (ParticleSystem effect in _handGlowEffects)
                effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
