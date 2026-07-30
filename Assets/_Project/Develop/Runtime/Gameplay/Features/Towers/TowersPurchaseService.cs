using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    public class TowersPurchaseService
    {
        private readonly ConfigsProviderService _configsProviderService;
        private readonly WalletService _wallet;

        private readonly TowersListConfig _towersListConfig;

        public TowersPurchaseService(
            ConfigsProviderService configsProviderService, 
            WalletService gameplayWallet)
        {
            _configsProviderService = configsProviderService;
            _wallet = gameplayWallet;

            _towersListConfig = _configsProviderService.GetConfig<TowersListConfig>();

            Debug.Log($"Начальное золото: {_wallet.GetCurrency(CurrencyTypes.Gold).Value}");
        }

        public bool TryBuyTower(TowerTypes type, int level)
        {
            TowerConfig towerConfig = _towersListConfig.GetBy(type, level);

            if (_wallet.Enough(CurrencyTypes.Gold, towerConfig.Cost) == false)
                return false;

            _wallet.Spend(CurrencyTypes.Gold, towerConfig.Cost);

            Debug.Log($"Башня куплена. Остаток золота: {_wallet.GetCurrency(CurrencyTypes.Gold).Value}");

            return true;
        }

        public bool EnoughGoldFor(TowerTypes type, int level)
        {
            TowerConfig towerConfig = _towersListConfig.GetBy(type, level);

            return _wallet.Enough(CurrencyTypes.Gold, towerConfig.Cost);
        }
    }
}
