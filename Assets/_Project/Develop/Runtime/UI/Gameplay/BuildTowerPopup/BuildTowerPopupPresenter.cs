using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Text;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    // Постройка на пустом месте: заряд кнопки ставит призрак башни, второе нажатие строит башню I уровня.
    public class BuildTowerPopupPresenter : TowerPopupPresenterBase<TowerTypes>
    {
        private const int TowersFirstLevel = 1;

        private readonly BuildTowerPopupView _view;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly TowersFactory _towersFactory;
        private readonly TowersListConfig _towersListConfig;

        private Entity _towerDemo;

        public BuildTowerPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            BuildTowerPopupView view,
            Entity towerPlaceholder,
            RayShooterService rayShooterService,
            EntitiesFactory entitiesFactory,
            TowersListConfig towersListConfig,
            TowersFactory towersFactory,
            TowersPurchaseService towersPurchaseService,
            WalletService walletService,
            TowersPlaceholdersService towersPlaceholdersService)
            : base(
                coroutinesPerformer,
                rayShooterService,
                view,
                towerPlaceholder,
                towersPurchaseService,
                towersPlaceholdersService,
                walletService)
        {
            _view = view;
            _entitiesFactory = entitiesFactory;
            _towersListConfig = towersListConfig;
            _towersFactory = towersFactory;
        }

        public override void Initialize()
        {
            base.Initialize();

            _view.ButtonClicked += OnButtonClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.ButtonClicked -= OnButtonClicked;

            ReleaseTowerDemo();
        }

        protected override void RefreshButtons()
        {
            foreach (TowerTypes type in Enum.GetValues(typeof(TowerTypes)))
                _view.SetButton(type, GetStateFor(type), TowersPurchaseService.GetPriceFor(type, TowersFirstLevel));
        }

        protected override bool IsAvailable(TowerTypes type) => TowersPurchaseService.EnoughGoldFor(type, TowersFirstLevel);

        protected override TowerTipContent GetTip(TowerTypes type)
        {
            ShootingTowerConfig config = _towersListConfig.GetBy(type, TowersFirstLevel);

            return new TowerTipContent(NonBreakingSpace.BeforeTrailingNumber(config.TowerName), config.TowerDesc);
        }

        protected override void OnArmed(TowerTypes type)
        {
            ShootingTowerConfig config = _towersListConfig.GetBy(type, TowersFirstLevel);

            _towerDemo = _entitiesFactory.CreateTowerDemo(Source.Transform.position, config);
        }

        protected override void OnDisarmed(TowerTypes type) => ReleaseTowerDemo();

        protected override void Activate(TowerTypes type)
        {
            if (TowersPurchaseService.TryBuyTower(type, TowersFirstLevel) == false)
                return;

            ShootingTowerConfig config = _towersListConfig.GetBy(type, TowersFirstLevel);

            ReleaseTowerDemo();
            TowersPlaceholdersService.ReleasePlaceholder(Source);
            _towersFactory.Create(Source.Transform.position, config, TowersFirstLevel);

            OnCloseRequest();
        }

        private void ReleaseTowerDemo()
        {
            if (_towerDemo == null)
                return;

            _towerDemo.SelfReleaseRequested.Value = true;
            _towerDemo = null;
        }
    }
}
