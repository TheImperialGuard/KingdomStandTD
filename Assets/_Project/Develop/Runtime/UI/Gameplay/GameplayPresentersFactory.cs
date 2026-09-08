using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet;
using Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerAbilitiesPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;

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

        public StartStagesPopupPresenter CreateStartStagesPopupPresenter(StartStagesPopupView view)
        {
            return new StartStagesPopupPresenter(
                view,
                _coroutinesPerformer,
                _container.Resolve<StagesCycle>(),
                _container.Resolve<RayShooterService>(),
                _container.Resolve<StageProviderService>(),
                _container.Resolve<MusicSwitcherService>());
        }

        public SkipStagePopupPresenter CreateSkipStagePopupPresenter(SkipStagePopupView view)
        {
            return new SkipStagePopupPresenter(
                _coroutinesPerformer,
                _container.Resolve<RayShooterService>(),
                view,
                _container.Resolve<StagesCycle>(),
                _container.Resolve<StageProviderService>());
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
                _container.Resolve<TowersFactory>(),
                _container.Resolve<TowersPurchaseService>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<TowersPlaceholdersService>());
        }

        public UpgradeTowerPopupPresenter CreateUpgradeTowerPopupPresenter(UpgradeTowerPopupView view, Entity sourceTower)
        {
            ConfigsProviderService configsProviderService = _container.Resolve<ConfigsProviderService>();
            TowersListConfig config = configsProviderService.GetConfig<TowersListConfig>();

            return new UpgradeTowerPopupPresenter(
                _coroutinesPerformer,
                _container.Resolve<RayShooterService>(),
                view,
                sourceTower,
                _container.Resolve<ResourcesAssetsLoader>(),
                _container.Resolve<TowersFactory>(),
                config,
                _container.Resolve<TowersPurchaseService>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<TowersPlaceholdersService>());
        }

        public TowerAbilitiesPopupPresenter CreateTowerAbilitiesPopupPresenter(TowerAbilitiesPopupView view, Entity sourceTower)
        {
            ConfigsProviderService configsProviderService = _container.Resolve<ConfigsProviderService>();
            TowersAbilitiesListConfig config = configsProviderService.GetConfig<TowersAbilitiesListConfig>();

            return new TowerAbilitiesPopupPresenter(
                _coroutinesPerformer,
                _container.Resolve<RayShooterService>(),
                view,
                sourceTower,
                config,
                _container.Resolve<TowersPurchaseService>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<TowersPlaceholdersService>(),
                _container.Resolve<AbilitiesFactory>());
        }

        public GameplayScreenPresenter CreateGameplayScreenPresenter(GameplayScreenView view)
        {
            return new GameplayScreenPresenter(view, this);
        }

        public GoldWalletPresenter CreateGoldWalletPresenter(IconTextView view)
        {
            return new GoldWalletPresenter(view, _container.Resolve<WalletService>());
        }
    }
}
