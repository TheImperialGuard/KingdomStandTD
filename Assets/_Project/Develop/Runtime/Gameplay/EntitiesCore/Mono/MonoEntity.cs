using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntity : MonoBehaviour
    {
        private CollidersRegistryService _colllidersRegistryService;
        private ParticleEffectsPool _particleEffectsPool;

        private Entity _linkedEntity;

        public Entity LinkedEntity => _linkedEntity;

        public void Initialize(CollidersRegistryService collidersRegistryService, ParticleEffectsPool particleEffectsPool)
        {
            _colllidersRegistryService = collidersRegistryService;
            _particleEffectsPool = particleEffectsPool;
        }

        public void Link(Entity entity)
        {
            _linkedEntity = entity;

            MonoEntityRegistrator[] registrators = GetComponentsInChildren<MonoEntityRegistrator>();

            if(registrators != null )
                foreach (MonoEntityRegistrator registrator in registrators)
                    registrator.Register(entity);

            EntityView[] views = GetComponentsInChildren<EntityView>();

            if (views != null )
                foreach (EntityView view in views)
                {
                    if (view is ParticleEffectsEntityView effectsView)
                        effectsView.SetEffectsPool(_particleEffectsPool);

                    view.Link(entity);
                }

            foreach (Collider collider in GetComponentsInChildren<Collider>())
                _colllidersRegistryService.Register(collider, entity);
        }

        public void Cleanup(Entity entity)
        {
            EntityView[] views = GetComponentsInChildren<EntityView>();

            if (views != null)
                foreach (EntityView view in views)
                    view.Cleanup(entity);

            foreach (Collider collider in GetComponentsInChildren<Collider>())
                _colllidersRegistryService.Unregister(collider);
        }
    }
}
