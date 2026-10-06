using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.SoundEffects;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class DeathSoundEffectView : SoundEffectViewBase
    {
        private IDisposable _isDeadChangedDisposable;

        protected override void Subscribe(Entity entity)
        {
            _isDeadChangedDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        protected override void Unsubscribe()
        {
            _isDeadChangedDisposable.Dispose();
        }

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead == false)
                return;

            PlaySound();
        }
    }
}
