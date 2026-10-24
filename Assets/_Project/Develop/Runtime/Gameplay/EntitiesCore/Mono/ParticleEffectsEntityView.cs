using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    // Вью, которая показывает эффекты частиц: берёт их из пула сцены, который выдаёт MonoEntity
    public abstract class ParticleEffectsEntityView : EntityView
    {
        protected ParticleEffectsPool EffectsPool { get; private set; }

        public void SetEffectsPool(ParticleEffectsPool effectsPool)
        {
            EffectsPool = effectsPool;
        }
    }
}
