using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
#endif

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    public class TowersPurchaseService
    {
        private const int FirstLevel = 1;

        private readonly ConfigsProviderService _configsProviderService;
        private readonly WalletService _wallet;

        private readonly TowersListConfig _towersListConfig;
        private readonly TowersAbilitiesListConfig _towersAbilitiesListConfig;

        public TowersPurchaseService(
            ConfigsProviderService configsProviderService, 
            WalletService gameplayWallet)
        {
            _configsProviderService = configsProviderService;
            _wallet = gameplayWallet;

            _towersListConfig = _configsProviderService.GetConfig<TowersListConfig>();
            _towersAbilitiesListConfig = _configsProviderService.GetConfig<TowersAbilitiesListConfig>();

            Debug.Log($"Начальное золото: {_wallet.GetCurrency(CurrencyTypes.Gold).Value}");
        }

        public bool TryBuyTower(TowerTypes type, int level)
        {
            ShootingTowerConfig towerConfig = _towersListConfig.GetBy(type, level);

            if (_wallet.Enough(CurrencyTypes.Gold, towerConfig.Cost) == false)
                return false;

            _wallet.Spend(CurrencyTypes.Gold, towerConfig.Cost);

            #if UNITY_EDITOR
                PlaytestHooks.NotifyTowerBought(type, level, towerConfig.Cost);
            #endif

            Debug.Log($"Башня куплена. Остаток золота: {_wallet.GetCurrency(CurrencyTypes.Gold).Value}");

            return true;
        }

        public void Sell(Entity tower)
        {
            int goldForSell = GetGoldAmountForSell(tower);

            _wallet.Add(CurrencyTypes.Gold, goldForSell);

            #if UNITY_EDITOR
                PlaytestHooks.NotifyTowerSold(tower.TowerType.Value, tower.TowerLevel.Value, goldForSell);
            #endif

            Debug.Log($"Башня продана. Текущее золото: {_wallet.GetCurrency(CurrencyTypes.Gold).Value}");
        }

        public int GetGoldAmountForSell(Entity tower)
        {
            GameConfig gameConfig = _configsProviderService.GetConfig<GameConfig>();

            int spentGold = GetSpentGoldForLevels(tower) + GetSpentGoldForAbilities(tower);

            int goldForSell = CalculateGoldForSell(spentGold, gameConfig.SellTowerGoldReturnIndex);

            return goldForSell;
        }

        public bool EnoughGoldFor(TowerTypes type, int level)
        {
            ShootingTowerConfig towerConfig = _towersListConfig.GetBy(type, level);

            return _wallet.Enough(CurrencyTypes.Gold, towerConfig.Cost);
        }

        public int GetPriceFor(TowerTypes type, int level)
        {
            ShootingTowerConfig towerConfig = _towersListConfig.GetBy(type, level);

            return towerConfig.Cost;
        }

        private int GetSpentGoldForLevels(Entity tower)
        {
            int spentGold = 0;

            for (int level = FirstLevel; level <= tower.TowerLevel.Value; level++)
                spentGold += GetPriceFor(tower.TowerType.Value, level);

            return spentGold;
        }

        private int GetSpentGoldForAbilities(Entity tower)
        {
            if (tower.TryGetAbilities(out AbilitiesList abilities) == false)
                return 0;

            TowersAbilitiesConfig towerAbilitiesConfig = _towersAbilitiesListConfig.GetBy(tower.TowerType.Value);

            int spentGold = 0;

            foreach (Ability ability in abilities.Elements)
            {
                AbilityConfig abilityConfig = GetAbilityConfigFor(ability, towerAbilitiesConfig);

                for (int level = FirstLevel; level <= ability.CurrentLevel.Value; level++)
                    spentGold += abilityConfig.GetCostBy(level);
            }

            return spentGold;
        }

        private AbilityConfig GetAbilityConfigFor(Ability ability, TowersAbilitiesConfig towerAbilitiesConfig)
        {
            return towerAbilitiesConfig.FirstAbilityGroup
                .Concat(towerAbilitiesConfig.SecondAbilityGroup)
                .First(config => config.ID == ability.ID);
        }

        private int CalculateGoldForSell(int cost, float sellTowerGoldReturnIndex)
        {
            float gold = (float)cost * sellTowerGoldReturnIndex;

            int finalGold = (int)Math.Round(gold, MidpointRounding.AwayFromZero);

            return finalGold;
        }
    }
}
