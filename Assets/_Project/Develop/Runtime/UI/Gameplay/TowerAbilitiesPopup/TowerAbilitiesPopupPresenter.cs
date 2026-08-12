using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerAbilitiesPopup
{
    public class TowerAbilitiesPopupPresenter : PopupPresenterBase
    {
        private readonly TowerAbilitiesPopupView _view;

        private readonly Entity _sourceTower;

        private readonly AbilitiesFactory _abilitiesFactory;
        private readonly TowersListConfig _towersListConfig;
        private readonly TowersPurchaseService _towersPurchaseService;
        private readonly WalletService _walletService;
        private readonly TowersPlaceholdersService _towersPlaceholdersService;

        private (AbilityConfig config, Ability ability) _currentFirstAbilityWithInfo;
        private (AbilityConfig config, Ability ability) _currentSecondAbilityWithInfo;

        private int _sellClicks;
        private int _firstAbilityClicks;
        private int _secondAbilityClicks;

        private IDisposable _goldCurrencyDisposable;

        public TowerAbilitiesPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            TowerAbilitiesPopupView view,
            Entity sourceTower,
            TowersListConfig towersListConfig,
            TowersPurchaseService towersPurchaseService,
            WalletService walletService,
            TowersPlaceholdersService towersPlaceholdersService,
            AbilitiesFactory abilitiesFactory) :
            base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _sourceTower = sourceTower;
            _towersListConfig = towersListConfig;
            _towersPurchaseService = towersPurchaseService;
            _walletService = walletService;
            _towersPlaceholdersService = towersPlaceholdersService;
            _abilitiesFactory = abilitiesFactory;
        }

        public TowerTypes TowerType => _sourceTower.TowerType.Value;

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.UpdateWorldPosition(_sourceTower.Transform.position);

            _view.FirstAbilityButtonClicked += OnFirstAbilityButtonClicked;
            _view.SecondAbilityButtonClicked += OnSecondAbilityButtonClicked;
            _view.SellButtonClicked += OnSellButtonClicked;

            _goldCurrencyDisposable = _walletService.GetCurrency(CurrencyTypes.Gold)
                .Subscribe(OnGoldChanged);

            _sellClicks = 0;
            _firstAbilityClicks = 0;
            _secondAbilityClicks = 0;

            SetCurrentAbilities();
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.FirstAbilityButtonClicked -= OnFirstAbilityButtonClicked;
            _view.SecondAbilityButtonClicked -= OnSecondAbilityButtonClicked;
            _view.SellButtonClicked -= OnSellButtonClicked;

            _goldCurrencyDisposable.Dispose();
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            _sellClicks = 0;
            _firstAbilityClicks = 0;
            _secondAbilityClicks = 0;

            OnCloseRequest();
        }

        private void OnFirstAbilityButtonClicked()
        {
            _sellClicks = 0;
            _secondAbilityClicks = 0;

            if (_firstAbilityClicks++ == 0)
                return;

            OnAbilitySelected(_currentFirstAbilityWithInfo);
            _firstAbilityClicks = 0;
        }

        private void OnSecondAbilityButtonClicked()
        {
            _sellClicks = 0;
            _firstAbilityClicks = 0;

            if (_secondAbilityClicks++ == 0)
                return;

            OnAbilitySelected(_currentSecondAbilityWithInfo);
            _secondAbilityClicks = 0;
        }

        private void OnAbilitySelected((AbilityConfig config, Ability ability) abilityWithInfo)
        {
            int abilityCost = GetAbilityCost(abilityWithInfo.ability, abilityWithInfo.config);

            if (_walletService.Enough(CurrencyTypes.Gold, abilityCost) == false)
                return;

            _walletService.Spend(CurrencyTypes.Gold, abilityCost);

            ProvideAbility(abilityWithInfo);

            _view.OnAbilityProvided();

            Debug.Log($"Получена способность: {abilityWithInfo.config.Name}");

            SetCurrentAbilities();
        }

        private void ProvideAbility((AbilityConfig config, Ability ability) abilityWithInfo)
        {
            if (abilityWithInfo.ability == null)
            {
                Ability newAbility = _abilitiesFactory.CreateAbilityFor(_sourceTower, abilityWithInfo.config, 1);
                _sourceTower.Abilities.Add(newAbility);
                return;
            }

            if (abilityWithInfo.ability.IsMaxLevel)
                throw new InvalidOperationException("Selected ability level is max");

            abilityWithInfo.ability.AddLevel(1);
        }

        private void SetCurrentAbilities()
        {
            TowerConfig config = _towersListConfig.GetBy(TowerType, _sourceTower.TowerLevel.Value);

            if (config is IHasAbilitiesTowerConfig hasAbilitiesTowerConfig == false)
                throw new ArgumentException("Tower config has not abilities support");

            IReadOnlyList<AbilityConfig> firsConfigsGroup = hasAbilitiesTowerConfig.FirstAbilityGroup;
            IReadOnlyList<AbilityConfig> secondConfigsGroup = hasAbilitiesTowerConfig.SecondAbilityGroup;

            _currentFirstAbilityWithInfo = GetCurrentAbilityWithInfo(firsConfigsGroup);
            _currentSecondAbilityWithInfo = GetCurrentAbilityWithInfo(secondConfigsGroup);

            SetupAbilityButtons();
        }

        private (AbilityConfig config, Ability ability) GetCurrentAbilityWithInfo(IReadOnlyList<AbilityConfig> configsGroup)
        {
            AbilityConfig config;
            Ability ability;

            if (configsGroup.Any() == false)
                throw new ArgumentException("Not found abilities group for tower");

            for (int i = 0; i < configsGroup.Count; i++)
            {
                config = configsGroup[i];

                ability = _sourceTower.Abilities.Elements.FirstOrDefault(abil => abil.ID == config.ID);

                if (ability == null || ability.IsMaxLevel == false)
                {
                    return (config, ability);
                }
            }

            config = configsGroup[configsGroup.Count - 1];

            ability = _sourceTower.Abilities.Elements.FirstOrDefault(abil => abil.ID == config.ID);

            return (config, ability);
        }

        private int GetAbilityCost(Ability ability, AbilityConfig config)
        {
            if (ability != null && ability.CurrentLevel.Value == config.MaxLevel)
                return config.LevelsCosts[config.LevelsCosts.Count - 1];

            int abilityCostIndex = ability == null ? 0 : ability.CurrentLevel.Value;
            int abilityCost = config.LevelsCosts[abilityCostIndex];

            return abilityCost;
        }

        private void OnSellButtonClicked()
        {
            _firstAbilityClicks = 0;
            _secondAbilityClicks = 0;

            if (_sellClicks++ == 0)
                return;

            _towersPurchaseService.Sell(TowerType, _sourceTower.TowerLevel.Value);

            CreateTowerPlaceholder();
            ReleaseSource();
            OnCloseRequest(false);
        }

        private void OnGoldChanged(int arg1, int arg2)
        {
            SetupAbilityButtons();
        }

        private bool IsAbilityAvailableForPurchase((AbilityConfig config, Ability ability) abilityWithInfo)
        {
            int abilityCost = GetAbilityCost(abilityWithInfo.ability, abilityWithInfo.config);

            if (_walletService.Enough(CurrencyTypes.Gold, abilityCost) == false)
                return false;

            if (abilityWithInfo.ability != null && abilityWithInfo.ability.CurrentLevel.Value == abilityWithInfo.config.MaxLevel)
                return false;

            return true;
        }

        private void SetupAbilityButtons()
        {
            if (IsAbilityAvailableForPurchase(_currentFirstAbilityWithInfo))
                _view.SwitchFirstAbilityInteractable(true);
            else
                _view.SwitchFirstAbilityInteractable(false);

            if (IsAbilityAvailableForPurchase(_currentSecondAbilityWithInfo))
                _view.SwitchSecondAbilityInteractable(true);
            else
                _view.SwitchSecondAbilityInteractable(false);
        }

        private void ReleaseSource() => _sourceTower.SelfReleaseRequested.Value = true;

        private void CreateTowerPlaceholder() => _towersPlaceholdersService.CreatePlaceholder(_sourceTower.Transform.position);
    }
}
