using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntitiesFactory : IInitializable, IDisposable
    {
        private const string PoolRootName = "MonoEntitiesPool";

        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;

        private readonly EntitiesLifeContext _entitiesLifeContext;

        private readonly CollidersRegistryService _collidersRegistryService;

        private readonly ParticleEffectsPool _particleEffectsPool;

        private readonly Dictionary<Entity, MonoEntity> _entityToMono = new();

        private readonly Dictionary<string, MonoEntity> _prefabs = new();

        private readonly Dictionary<string, Stack<MonoEntity>> _freeInstances = new();
        private readonly Dictionary<MonoEntity, Stack<MonoEntity>> _instanceToFreeInstances = new();

        private Transform _poolRoot;

        public MonoEntitiesFactory(
            ResourcesAssetsLoader resourcesAssetsLoader,
            EntitiesLifeContext entitiesLifeContext,
            CollidersRegistryService collidersRegistryService,
            ParticleEffectsPool particleEffectsPool)
        {
            _resourcesAssetsLoader = resourcesAssetsLoader;
            _entitiesLifeContext = entitiesLifeContext;
            _collidersRegistryService = collidersRegistryService;
            _particleEffectsPool = particleEffectsPool;
        }

        public MonoEntity Create(Entity entity, Vector3 position, string path)
        {
            MonoEntity viewInstance = CreateInstance(path, position);

            Link(entity, viewInstance);

            return viewInstance;
        }

        // Объект берётся из пула, а при освобождении сущности возвращается туда вместо Destroy.
        // Префаб должен быть готов к повторному Link: коллайдеры и скорость Rigidbody сбрасывает MonoEntity,
        // остальное, что вью меняют в объекте, они отменяют в Cleanup или выставляют заново при старте сущности
        public MonoEntity CreatePooled(Entity entity, Vector3 position, string path)
        {
            Stack<MonoEntity> freeInstances = GetFreeInstances(path);

            if (freeInstances.TryPop(out MonoEntity viewInstance))
            {
                PlaceFromPool(viewInstance, position);
            }
            else
            {
                viewInstance = CreateInstance(path, position);
                _instanceToFreeInstances.Add(viewInstance, freeInstances);
            }

            Link(entity, viewInstance);

            return viewInstance;
        }

        public void Initialize()
        {
            _poolRoot = new GameObject(PoolRootName).transform;

            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public void Dispose()
        {
            _entitiesLifeContext.Released -= OnEntityReleased;

            foreach (Entity entity in _entityToMono.Keys)
                CleanupFor(entity);

            _entityToMono.Clear();

            if (_poolRoot != null)
                GameObject.Destroy(_poolRoot.gameObject);

            _freeInstances.Clear();
            _instanceToFreeInstances.Clear();
        }

        private MonoEntity CreateInstance(string path, Vector3 position)
        {
            MonoEntity viewInstance = GameObject.Instantiate(GetPrefab(path), position, Quaternion.identity, null);

            viewInstance.Initialize(_collidersRegistryService, _particleEffectsPool);

            return viewInstance;
        }

        private MonoEntity GetPrefab(string path)
        {
            if (_prefabs.TryGetValue(path, out MonoEntity prefab) == false)
            {
                prefab = _resourcesAssetsLoader.Load<MonoEntity>(path);
                _prefabs.Add(path, prefab);
            }

            return prefab;
        }

        private Stack<MonoEntity> GetFreeInstances(string path)
        {
            if (_freeInstances.TryGetValue(path, out Stack<MonoEntity> freeInstances) == false)
            {
                freeInstances = new Stack<MonoEntity>();
                _freeInstances.Add(path, freeInstances);
            }

            return freeInstances;
        }

        // Позиция ставится, пока объект выключен: при включении коллайдеры и Rigidbody
        // появляются в физике сразу на новом месте, как у свежего экземпляра
        private void PlaceFromPool(MonoEntity viewInstance, Vector3 position)
        {
            Transform instanceTransform = viewInstance.transform;

            instanceTransform.SetParent(null, false);
            instanceTransform.SetPositionAndRotation(position, Quaternion.identity);

            viewInstance.gameObject.SetActive(true);
        }

        private void Link(Entity entity, MonoEntity viewInstance)
        {
            viewInstance.Link(entity);

            _entityToMono.Add(entity, viewInstance);
        }

        private void OnEntityReleased(Entity entity)
        {
            CleanupFor(entity);

            _entityToMono.Remove(entity);
        }

        private void CleanupFor(Entity entity)
        {
            if (_entityToMono.TryGetValue(entity, out MonoEntity monoEntity) == false)
                return;

            monoEntity.Cleanup(entity);

            if (_instanceToFreeInstances.TryGetValue(monoEntity, out Stack<MonoEntity> freeInstances))
                ReturnToPool(monoEntity, freeInstances);
            else
                GameObject.Destroy(monoEntity.gameObject);
        }

        private void ReturnToPool(MonoEntity monoEntity, Stack<MonoEntity> freeInstances)
        {
            monoEntity.ResetForReuse();

            monoEntity.gameObject.SetActive(false);
            monoEntity.transform.SetParent(_poolRoot, false);

            freeInstances.Push(monoEntity);
        }
    }
}
