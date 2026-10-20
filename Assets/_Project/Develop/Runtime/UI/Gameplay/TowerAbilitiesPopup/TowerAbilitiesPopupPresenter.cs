using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
#endif

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerAbilitiesPopup
{
    // Способности башни IV уровня и продажа. После покупки уровня способности окно остаётся открытым (Р-37): заряд
    // снимается, кнопки показывают новый уровень.
    public class TowerAbilitiesPopupPresenter : TowerPopupPresenterBase<TowerAbilitiesPopupButtons>
    {
        private const string LevelPrefix = "Уровень ";
        private const string NextLevelArrow = " → ";

        private readonly TowerAbilitiesPopupView _view;

        private readonly AbilitiesFactory _abilitiesFactory;
        private readonly TowersAbilitiesListConfig _towersAbilitiesListConfig;
        private readonly WalletService _walletService;

        private (AbilityConfig config, Ability ability) _currentFirstAbilityWithInfo;
        private (AbilityConfig config, Ability ability) _currentSecondAbilityWithInfo;

        public TowerAbilitiesPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            TowerAbilitiesPopupView view,
            Entity sourceTower,
            TowersAbilitiesListConfig towersAbilitiesListConfig,
            TowersPurchaseService towersPurchaseService,
            WalletService walletService,
            TowersPlaceholdersService towersPlaceholdersService,
            AbilitiesFactory abilitiesFactory)
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
            _towersAbilitiesListConfig = towersAbilitiesListConfig;
            _walletService = walletService;
            _abilitiesFactory = abilitiesFactory;
        }

        public TowerTypes TowerType => Source.TowerType.Value;

        public override void Initialize()
        {
            SetCurrentAbilities();

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
            RefreshAbilityButton(TowerAbilitiesPopupButtons.FirstAbility);
            RefreshAbilityButton(TowerAbilitiesPopupButtons.SecondAbility);

            _view.SetSellButton(GetStateFor(TowerAbilitiesPopupButtons.Sell));
        }

        protected override bool IsAvailable(TowerAbilitiesPopupButtons button)
        {
            if (button == TowerAbilitiesPopupButtons.Sell)
                return true;

            (AbilityConfig config, Ability ability) abilityWithInfo = GetAbilityWithInfo(button);

            if (IsAbilityOnMaxLevel(abilityWithInfo))
                return false;

            return _walletService.Enough(CurrencyTypes.Gold, GetAbilityCost(abilityWithInfo));
        }

        protected override bool IsMaxLevel(TowerAbilitiesPopupButtons button)
        {
            if (button == TowerAbilitiesPopupButtons.Sell)
                return false;

            return IsAbilityOnMaxLevel(GetAbilityWithInfo(button));
        }

        protected override TowerTipContent GetTip(TowerAbilitiesPopupButtons button)
        {
            if (button == TowerAbilitiesPopupButtons.Sell)
                return GetSellTip();

            (AbilityConfig config, Ability ability) abilityWithInfo = GetAbilityWithInfo(button);

            int level = GetAbilityLevel(abilityWithInfo);
            int nextLevel = level + 1;

            return new TowerTipContent(
                abilityWithInfo.config.Name,
                abilityWithInfo.config.DescriptionByLevel[level],
                subtitle: LevelPrefix + level + NextLevelArrow + nextLevel,
                parameters: EncyclopediaAbilityNumbers.Format(abilityWithInfo.config, nextLevel));
        }

        protected override void Activate(TowerAbilitiesPopupButtons button)
        {
            if (button == TowerAbilitiesPopupButtons.Sell)
            {
                SellSource();
                return;
            }

            BuyAbilityLevel(GetAbilityWithInfo(button));
        }

        private void BuyAbilityLevel((AbilityConfig config, Ability ability) abilityWithInfo)
        {
            int abilityCost = GetAbilityCost(abilityWithInfo);

            if (_walletService.Enough(CurrencyTypes.Gold, abilityCost) == false)
                return;

            _walletService.Spend(CurrencyTypes.Gold, abilityCost);

            #if UNITY_EDITOR
                // Покупка способности — в лог плейтеста из редактора уровней
                PlaytestHooks.NotifyAbilityBought(TowerType, abilityWithInfo.config, abilityCost);
            #endif

            ProvideAbility(abilityWithInfo);

            Debug.Log($"Получена способность: {abilityWithInfo.config.Name}");

            SetCurrentAbilities();
            Disarm();
        }

        private void ProvideAbility((AbilityConfig config, Ability ability) abilityWithInfo)
        {
            if (abilityWithInfo.ability == null)
            {
                Ability newAbility = _abilitiesFactory.CreateAbilityFor(Source, abilityWithInfo.config, 1);
                Source.Abilities.Add(newAbility);
                Debug.Log($"Создана спосоьность уровня: {newAbility.CurrentLevel.Value}");
                Debug.Log($"Макс уровень: {newAbility.MaxLevel}");
                return;
            }

            if (abilityWithInfo.ability.IsMaxLevel)
                throw new InvalidOperationException("Selected ability level is max");

            abilityWithInfo.ability.AddLevel(1);
        }

        private void RefreshAbilityButton(TowerAbilitiesPopupButtons button)
        {
            (AbilityConfig config, Ability ability) abilityWithInfo = GetAbilityWithInfo(button);

            _view.SetAbilityIcon(button, abilityWithInfo.config.Icon);
            _view.SetAbilityButton(button, GetStateFor(button), GetAbilityCost(abilityWithInfo));
            _view.SetAbilityLevels(button, GetAbilityLevel(abilityWithInfo), abilityWithInfo.config.MaxLevel);
        }

        private (AbilityConfig config, Ability ability) GetAbilityWithInfo(TowerAbilitiesPopupButtons button)
        {
            return button switch
            {
                TowerAbilitiesPopupButtons.FirstAbility => _currentFirstAbilityWithInfo,
                TowerAbilitiesPopupButtons.SecondAbility => _currentSecondAbilityWithInfo,
                _ => throw new ArgumentOutOfRangeException(nameof(button)),
            };
        }

        private void SetCurrentAbilities()
        {
            TowersAbilitiesConfig config = _towersAbilitiesListConfig.GetBy(TowerType);

            _currentFirstAbilityWithInfo = GetCurrentAbilityWithInfo(config.FirstAbilityGroup);
            _currentSecondAbilityWithInfo = GetCurrentAbilityWithInfo(config.SecondAbilityGroup);
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

                ability = Source.Abilities.Elements.FirstOrDefault(abil => abil.ID == config.ID);

                if (ability == null || ability.IsMaxLevel == false)
                {
                    return (config, ability);
                }
            }

            config = configsGroup[configsGroup.Count - 1];

            ability = Source.Abilities.Elements.FirstOrDefault(abil => abil.ID == config.ID);

            return (config, ability);
        }

        private int GetAbilityCost((AbilityConfig config, Ability ability) abilityWithInfo)
        {
            if (IsAbilityOnMaxLevel(abilityWithInfo))
                return abilityWithInfo.config.LevelsCosts[abilityWithInfo.config.LevelsCosts.Count - 1];

            return abilityWithInfo.config.LevelsCosts[GetAbilityLevel(abilityWithInfo)];
        }

        private int GetAbilityLevel((AbilityConfig config, Ability ability) abilityWithInfo)
            => abilityWithInfo.ability == null ? 0 : abilityWithInfo.ability.CurrentLevel.Value;

        private bool IsAbilityOnMaxLevel((AbilityConfig config, Ability ability) abilityWithInfo)
            => abilityWithInfo.ability != null && abilityWithInfo.ability.CurrentLevel.Value == abilityWithInfo.config.MaxLevel;
    }
}
