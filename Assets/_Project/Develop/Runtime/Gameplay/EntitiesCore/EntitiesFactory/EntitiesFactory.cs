using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Infrastructure.DI;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        private readonly DIContainer _container;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly CollidersRegistryService _collidersRegistryService;

        private readonly MonoEntitiesFactory _monoEntitiesFactory;
        private readonly InteractiveActionsFactory _interactiveActionsFactory;

        public EntitiesFactory(DIContainer container)
        {
            _container = container;
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = _container.Resolve<MonoEntitiesFactory>();
            _collidersRegistryService = _container.Resolve<CollidersRegistryService>();
            _interactiveActionsFactory = _container.Resolve<InteractiveActionsFactory>();
        }

        private Entity CreateEmpty() => new Entity();
    }
}
