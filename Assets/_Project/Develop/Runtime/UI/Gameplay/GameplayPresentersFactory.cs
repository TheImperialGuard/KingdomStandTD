using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private readonly DIContainer _container;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public GameplayPresentersFactory(DIContainer container)
        {
            _container = container;
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
        }

        public BuildTowerPopupPresenter CreateBuildTowerPopupPresenter(BuildTowerPopupView view, Entity towerPlaceholder)
        {
            ConfigsProviderService configsProviderService = _container.Resolve<ConfigsProviderService>();
            TowersListConfig config = configsProviderService.GetConfig<TowersListConfig>();

            return new BuildTowerPopupPresenter(
                _coroutinesPerformer,
                view,
                towerPlaceholder,
                _container.Resolve<RayShooterService>(),
                _container.Resolve<EntitiesFactory>(),
                config,
                _container.Resolve<TowersFactory>());
        }
    }
}
