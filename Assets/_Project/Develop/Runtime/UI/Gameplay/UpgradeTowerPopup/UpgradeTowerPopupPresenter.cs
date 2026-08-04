using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup
{
    public class UpgradeTowerPopupPresenter : PopupPresenterBase
    {
        private const string RangeZonePrefabPath = "Prefabs/Entities/Towers/ShootingRangeZone";

        private readonly UpgradeTowerPopupView _view;

        private readonly Entity _sourceTower;

        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;
        private readonly TowersFactory _towersFactory;
        private readonly TowersListConfig _towersListConfig;
        private readonly TowersPurchaseService _towersPurchaseService;
        private readonly WalletService _walletService;
        private readonly TowersPlaceholdersService _towersPlaceholdersService;

        private ShootingRangeZone _rangeDemo;
        private int _sellClicks;

        private IDisposable _goldCurrencyDisposable;

        public UpgradeTowerPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            UpgradeTowerPopupView view, 
            Entity sourceTower, 
            ResourcesAssetsLoader resourcesAssetsLoader, 
            TowersFactory towersFactory, 
            TowersListConfig towersListConfig, 
            TowersPurchaseService towersPurchaseService, 
            WalletService walletService, 
            TowersPlaceholdersService towersPlaceholdersService) :
            base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _sourceTower = sourceTower;
            _resourcesAssetsLoader = resourcesAssetsLoader;
            _towersFactory = towersFactory;
            _towersListConfig = towersListConfig;
            _towersPurchaseService = towersPurchaseService;
            _walletService = walletService;
            _towersPlaceholdersService = towersPlaceholdersService;
        }

        public TowerTypes TowerType => _sourceTower.TowerType.Value;

        protected override PopupViewBase PopupView => _view;

        private int NextTowerLevel => _sourceTower.TowerLevel.Value + 1;

        public override void Initialize()
        {
            base.Initialize();

            _view.UpdateWorldPosition(_sourceTower.Transform.position);

            _view.UpgradeButtonClicked += OnUpgradeButtonClicked;
            _view.SellButtonClicked += OnSellButtonClicked;

            _goldCurrencyDisposable = _walletService.GetCurrency(CurrencyTypes.Gold)
                .Subscribe(OnGoldChanged);

            OnGoldChanged(0, 0);

            _sellClicks = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.UpgradeButtonClicked += OnUpgradeButtonClicked;
            _view.SellButtonClicked += OnSellButtonClicked;

            _goldCurrencyDisposable.Dispose();

            ReleaseRangeDemo();
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            _sellClicks = 0;

            ReleaseRangeDemo();
            OnCloseRequest();
        }

        private void OnUpgradeButtonClicked()
        {
            _sellClicks = 0;

            TowerConfig config = _towersListConfig.GetBy(TowerType, NextTowerLevel);

            if (_rangeDemo == null)
            {
                CreateRangeDemo(config);
                return;
            }

            if (_towersPurchaseService.TryBuyTower(TowerType, NextTowerLevel) == false)
                return;

            UpgradeTower(config);
            OnCloseRequest();
        }

        private void OnSellButtonClicked()
        {
            ReleaseRangeDemo();

            if (_sellClicks++ == 0)
                return;

            _towersPurchaseService.Sell(TowerType, _sourceTower.TowerLevel.Value);

            CreateTowerPlaceholder();
            ReleaseSource();
            OnCloseRequest(false);
        }

        private void OnGoldChanged(int arg1, int arg2)
        {
            if (_towersPurchaseService.EnoughGoldFor(TowerType, NextTowerLevel))
                _view.SwitchUpgradeInteractable(true);
            else
                _view.SwitchUpgradeInteractable(false);
        }

        private void UpgradeTower(TowerConfig config)
        {
            ReleaseRangeDemo();

            _towersFactory.Create(_sourceTower.Transform.position, config, NextTowerLevel);

            ReleaseSource();
            OnCloseRequest(false);
        }

        private void CreateRangeDemo(TowerConfig config)
        {
            ShootingRangeZone zonePrefab = _resourcesAssetsLoader.Load<ShootingRangeZone>(RangeZonePrefabPath);

            _rangeDemo = GameObject.Instantiate(zonePrefab, _sourceTower.Transform.position, Quaternion.identity);

            _rangeDemo.SetRange(config.AttackRange);
            _rangeDemo.Show();
        }

        private void ReleaseRangeDemo()
        {
            if (_rangeDemo != null)
                GameObject.Destroy(_rangeDemo.gameObject);

            _rangeDemo = null;
        }

        private void ReleaseSource() => _sourceTower.SelfReleaseRequested.Value = true;

        private void CreateTowerPlaceholder() => _towersPlaceholdersService.CreatePlaceholder(_sourceTower.Transform.position);
    }
}
