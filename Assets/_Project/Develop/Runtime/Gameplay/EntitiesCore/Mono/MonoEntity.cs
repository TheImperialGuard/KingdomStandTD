using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntity : MonoBehaviour
    {
        private CollidersRegistryService _colllidersRegistryService;
        private ParticleEffectsPool _particleEffectsPool;

        private Entity _linkedEntity;

        // Части объекта ищутся один раз, на первом Link: Cleanup идёт ровно по тем же частям,
        // а объект из пула при повторном Link не ищет их заново
        private MonoEntityRegistrator[] _registrators;
        private EntityView[] _views;
        private Collider[] _colliders;
        private bool[] _initialCollidersEnabled;
        private Rigidbody[] _rigidbodies;

        public Entity LinkedEntity => _linkedEntity;

        public void Initialize(CollidersRegistryService collidersRegistryService, ParticleEffectsPool particleEffectsPool)
        {
            _colllidersRegistryService = collidersRegistryService;
            _particleEffectsPool = particleEffectsPool;
        }

        public void Link(Entity entity)
        {
            _linkedEntity = entity;

            if (_views == null)
                CacheParts();

            foreach (MonoEntityRegistrator registrator in _registrators)
                registrator.Register(entity);

            foreach (EntityView view in _views)
            {
                if (view is ParticleEffectsEntityView effectsView)
                    effectsView.SetEffectsPool(_particleEffectsPool);

                view.Link(entity);
            }

            foreach (Collider collider in _colliders)
                _colllidersRegistryService.Register(collider, entity);
        }

        public void Cleanup(Entity entity)
        {
            foreach (EntityView view in _views)
                view.Cleanup(entity);

            foreach (Collider collider in _colliders)
                _colllidersRegistryService.Unregister(collider);
        }

        // Возвращает то, что системы меняют в объекте за жизнь сущности, к виду свежего экземпляра.
        // Вызывается до выключения объекта, пока его Rigidbody ещё в физической сцене
        public void ResetForReuse()
        {
            for (int i = 0; i < _colliders.Length; i++)
                _colliders[i].enabled = _initialCollidersEnabled[i];

            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                if (rigidbody.isKinematic == true)
                    continue;

                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }

        private void CacheParts()
        {
            _registrators = GetComponentsInChildren<MonoEntityRegistrator>();
            _views = GetComponentsInChildren<EntityView>();
            _colliders = GetComponentsInChildren<Collider>();
            _rigidbodies = GetComponentsInChildren<Rigidbody>();

            _initialCollidersEnabled = new bool[_colliders.Length];

            for (int i = 0; i < _colliders.Length; i++)
                _initialCollidersEnabled[i] = _colliders[i].enabled;
        }
    }
}
