using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Text;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup
{
    // Улучшение башни I–III уровня и продажа. Заряд улучшения показывает на круге дальности башни прирост до дальности
    // следующего уровня (Р-19).
    public class UpgradeTowerPopupPresenter : TowerPopupPresenterBase<UpgradeTowerPopupButtons>
    {
        private readonly UpgradeTowerPopupView _view;

        private readonly TowersFactory _towersFactory;
        private readonly TowersListConfig _towersListConfig;

        public UpgradeTowerPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            UpgradeTowerPopupView view,
            Entity sourceTower,
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
                Source.ShootingRangeZone.ShowGrowth(NextLevelConfig.AttackRange);
        }

        protected override void OnDisarmed(UpgradeTowerPopupButtons button) => Source.ShootingRangeZone.HideGrowth();

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

            _towersFactory.Create(Source.Transform.position, NextLevelConfig, NextTowerLevel);

            Source.SelfReleaseRequested.Value = true;
            OnCloseRequest(false);
        }
    }
}
