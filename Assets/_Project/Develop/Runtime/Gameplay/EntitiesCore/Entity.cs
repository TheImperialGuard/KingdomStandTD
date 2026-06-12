using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public partial class Entity : IDisposable
    {
        public event Action<Entity> Initialized;

        private readonly Dictionary<Type, IEntityComponent> _components = new();

        private readonly List<IEntitySystem> _systems = new();

        private readonly List<IInitializableSystem> _initializableSystems = new();
        private readonly List<IUpdatableSystem> _updatableSystems = new();
        private readonly List<IDisposableSystem> _disposableSystems = new();

        private bool _isInit;

        public bool IsInit => _isInit;

        public void Initialize()
        {
            foreach (IInitializableSystem system in _initializableSystems)
                system.OnInit(this);

            _isInit = true;

            Initialized?.Invoke(this);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isInit == false)
                return;

            foreach (IUpdatableSystem system in _updatableSystems)
                system.OnUpdate(deltaTime);
        }

        public void Dispose()
        {
            foreach (IDisposableSystem system in _disposableSystems)
                system.OnDispose();

            _isInit = false;
        }

        public Entity AddComponent<TComponent>(TComponent component) where TComponent : class, IEntityComponent
        {
            _components.Add(typeof(TComponent), component);
            return this;
        }

        public bool HasComponent<TComponent>() where TComponent : class, IEntityComponent
        {
            return _components.ContainsKey(typeof(TComponent));
        }

        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : class, IEntityComponent
        {
            if (_components.TryGetValue(typeof(TComponent), out IEntityComponent findedObject))
            {
                component = (TComponent)findedObject;
                return true;
            }

            component = null;
            return false;
        }

        public TComponent GetComponent<TComponent>() where TComponent : class, IEntityComponent
        {
            if (TryGetComponent(out TComponent component) == false)
                throw new ArgumentException($"Entity not exist {typeof(TComponent)}");

            return component;
        }

        public Entity AddSystem(IEntitySystem system)
        {
            if (_systems.Contains(system))
                throw new ArgumentException(system.GetType().ToString());

            _systems.Add(system);

            if (system is IInitializableSystem initializableSystem)
            {
                _initializableSystems.Add(initializableSystem);

                if (_isInit)
                    initializableSystem.OnInit(this);
            }

            if (system is IUpdatableSystem updatableSystem)
                _updatableSystems.Add(updatableSystem);

            if (system is IDisposableSystem disposableSystem)
                _disposableSystems.Add(disposableSystem);

            return this;
        }
    }
}
