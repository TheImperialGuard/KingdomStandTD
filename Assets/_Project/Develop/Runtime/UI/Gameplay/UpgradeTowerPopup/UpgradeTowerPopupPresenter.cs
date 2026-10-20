using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Text;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup
{
    // Улучшение башни I–III уровня и продажа. Заряд улучшения показывает поверх текущего круга дальности круг следующего
    // уровня (Р-19).
    public class UpgradeTowerPopupPresenter : TowerPopupPresenterBase<UpgradeTowerPopupButtons>
    {
        private const string RangeZonePrefabPath = "Prefabs/Entities/Towers/ShootingRangeZone";

        private readonly UpgradeTowerPopupView _view;

        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;
        private readonly TowersFactory _towersFactory;
        private readonly TowersListConfig _towersListConfig;

        private ShootingRangeZone _rangeDemo;

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
            TowersPlaceholdersService towersPlaceholdersService)
            : base(
                coroutinesPerformer,
                rayShooterService,
                view,
                sourceTower,
                towersPurchaseService,
                towersPlaceholdersService,
                walletService)
        {
            _view = view;
            _resourcesAssetsLoader = resourcesAssetsLoader;
            _towersFactory = towersFactory;
            _towersListConfig = towersListConfig;
        }

        public TowerTypes TowerType => Source.TowerType.Value;

        private int NextTowerLevel => Source.TowerLevel.Value + 1;

        private ShootingTowerConfig NextLevelConfig => _towersListConfig.GetBy(TowerType, NextTowerLevel);

        public override void Initialize()
        {
            base.Initialize();

            _view.ButtonClicked += OnButtonClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.ButtonClicked -= OnButtonClicked;

            ReleaseRangeDemo();
        }

        protected override void RefreshButtons()
        {
            int upgradeCost = TowersPurchaseService.GetPriceFor(TowerType, NextTowerLevel);

            _view.SetUpgradeButton(GetStateFor(UpgradeTowerPopupButtons.Upgrade), upgradeCost);
            _view.SetSellButton(GetStateFor(UpgradeTowerPopupButtons.Sell));
        }

        protected override bool IsAvailable(UpgradeTowerPopupButtons button)
        {
            switch (button)
            {
                case UpgradeTowerPopupButtons.Upgrade:
                    return TowersPurchaseService.EnoughGoldFor(TowerType, NextTowerLevel);

                case UpgradeTowerPopupButtons.Sell:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(button));
            }
        }

        protected override TowerTipContent GetTip(UpgradeTowerPopupButtons button)
        {
            switch (button)
            {
                case UpgradeTowerPopupButtons.Upgrade:
                    return new TowerTipContent(
                        NonBreakingSpace.BeforeTrailingNumber(NextLevelConfig.TowerName),
                        NextLevelConfig.TowerDesc);

                case UpgradeTowerPopupButtons.Sell:
                    return GetSellTip();

                default:
                    throw new ArgumentOutOfRangeException(nameof(button));
            }
        }

        protected override void OnArmed(UpgradeTowerPopupButtons button)
        {
            if (button == UpgradeTowerPopupButtons.Upgrade)
                CreateRangeDemo(NextLevelConfig);
        }

        protected override void OnDisarmed(UpgradeTowerPopupButtons button) => ReleaseRangeDemo();

        protected override void Activate(UpgradeTowerPopupButtons button)
        {
            switch (button)
            {
                case UpgradeTowerPopupButtons.Upgrade:
                    UpgradeTower();
                    break;

                case UpgradeTowerPopupButtons.Sell:
                    SellSource();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(button));
            }
        }

        private void UpgradeTower()
        {
            if (TowersPurchaseService.TryBuyTower(TowerType, NextTowerLevel) == false)
                return;

            ReleaseRangeDemo();

            _towersFactory.Create(Source.Transform.position, NextLevelConfig, NextTowerLevel);

            Source.SelfReleaseRequested.Value = true;
            OnCloseRequest(false);
        }

        private void CreateRangeDemo(ShootingTowerConfig config)
        {
            ShootingRangeZone zonePrefab = _resourcesAssetsLoader.Load<ShootingRangeZone>(RangeZonePrefabPath);

            _rangeDemo = GameObject.Instantiate(zonePrefab, Source.Transform.position, Quaternion.identity);

            _rangeDemo.SetRange(config.AttackRange);
            _rangeDemo.Show();
        }

        private void ReleaseRangeDemo()
        {
            if (_rangeDemo != null)
                GameObject.Destroy(_rangeDemo.gameObject);

            _rangeDemo = null;
        }
    }
}
