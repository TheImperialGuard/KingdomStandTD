using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.SoundEffects;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackSoundEffectView : SoundEffectViewBase
    {
        private IDisposable _attackDelayEndEventDisposable;

        protected override void Subscribe(Entity entity)
        {
            _attackDelayEndEventDisposable = entity.AttackDelayEndEvent.Subscribe(PlaySound);
        }

        protected override void Unsubscribe()
        {
            _attackDelayEndEventDisposable.Dispose();
        }
    }
}
