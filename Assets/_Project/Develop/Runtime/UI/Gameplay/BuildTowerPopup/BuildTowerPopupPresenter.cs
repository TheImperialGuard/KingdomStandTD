using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    public class BuildTowerPopupPresenter : PopupPresenterBase
    {
        private const int TowersFirstLevel = 1;

        private readonly BuildTowerPopupView _view;

        private readonly Entity _towerPlaceholder;
        private readonly EntitiesFactory _entitiesFactory;
        private readonly TowersFactory _towersFactory;
        private readonly TowersListConfig _towersListConfig;
        private readonly TowersPurchaseService _towersPurchaseService;
        private readonly WalletService _walletService;

        private Entity _createdTowerDemo;
        private TowerTypes _createdTowerDemoType;
        private IDisposable _goldCurrencyDisposable;

        public BuildTowerPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            BuildTowerPopupView view,
            Entity towerPlaceholder,
            RayShooterService rayShooterService,
            EntitiesFactory entitiesFactory,
            TowersListConfig towersListConfig,
            TowersFactory towersFactory,
            TowersPurchaseService towersPurchaseService,
            WalletService walletService)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _towerPlaceholder = towerPlaceholder;
            _entitiesFactory = entitiesFactory;
            _towersListConfig = towersListConfig;
            _towersFactory = towersFactory;
            _towersPurchaseService = towersPurchaseService;
            _walletService = walletService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.UpdatePosition(_towerPlaceholder.Transform.position);

            _view.BuildTowerButtonClicked += OnBuildTowerButtonClicked;

            _goldCurrencyDisposable = _walletService.GetCurrency(CurrencyTypes.Gold)
                .Subscribe(OnGoldChanged);

            OnGoldChanged(0, 0);
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.BuildTowerButtonClicked -= OnBuildTowerButtonClicked;
            _goldCurrencyDisposable.Dispose();

            ReleaseCurrentDemo();
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            ReleaseCurrentDemo();
            OnCloseRequest();
        }

        private void OnBuildTowerButtonClicked(TowerTypes type)
        {
            TowerConfig config = _towersListConfig.GetBy(type, TowersFirstLevel);

            if (_createdTowerDemo == null || type != _createdTowerDemoType)
            {
                CreateTowerDemo(config);
                _createdTowerDemoType = type;
                return;
            }

            if (_towersPurchaseService.TryBuyTower(type, TowersFirstLevel) == false)
                return;

            BuildTower(config);
            OnCloseRequest();
        }

        private void BuildTower(TowerConfig config)
        {
            ReleaseCurrentDemo();
            ReleaseTowerPlaceholder();
            _towersFactory.Create(_towerPlaceholder.Transform.position, config);
        }

        private void ReleaseTowerPlaceholder()
        {
            _towerPlaceholder.SelfReleaseRequested.Value = true; 
        }

        private void CreateTowerDemo(TowerConfig config)
        {
            ReleaseCurrentDemo();
            _createdTowerDemo = _entitiesFactory.CreateTowerDemo(_towerPlaceholder.Transform.position, config);
        }

        private void ReleaseCurrentDemo()
        {
            if (_createdTowerDemo != null)
            {
                _createdTowerDemo.SelfReleaseRequested.Value = true;
            }
        }

        private void OnGoldChanged(int oldValue, int newValue)
        {
            foreach (TowerTypes type in Enum.GetValues(typeof(TowerTypes)))
            {
                if (_towersPurchaseService.EnoughGoldFor(type, TowersFirstLevel))
                    _view.SwitchInteractableFor(type, true);
                else
                    _view.SwitchInteractableFor(type, false);
            }
        }
    }
}
