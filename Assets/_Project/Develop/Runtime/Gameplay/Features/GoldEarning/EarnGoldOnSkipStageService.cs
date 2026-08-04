using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning
{
    public class EarnGoldOnSkipStageService
    {
        private readonly ConfigsProviderService _configProviderService;
        private readonly WalletService _walletService;

        private readonly GameConfig _gameConfig;

        public EarnGoldOnSkipStageService(
            ConfigsProviderService configProviderService, 
            WalletService walletService)
        {
            _configProviderService = configProviderService;
            _walletService = walletService;

            _gameConfig = _configProviderService.GetConfig<GameConfig>();
        }

        public void Earn(float remainingStageTime)
        {
            if (remainingStageTime < 0f)
                throw new ArgumentOutOfRangeException(nameof(remainingStageTime));

            int earnedGold = CalculateEarnedGold(remainingStageTime);

            _walletService.Add(CurrencyTypes.Gold, earnedGold);

            Debug.Log($"Золото за скип: {earnedGold}. Всего: {_walletService.GetCurrency(CurrencyTypes.Gold).Value}");
        }

        private int CalculateEarnedGold(float remainingStageTime)
        {
            float goldIndex = _gameConfig.GoldIndexForRemainingStageSeconds;
            int maxGold = _gameConfig.MaxGoldForSkipStage;

            float earnedGold = remainingStageTime * goldIndex;

            int finalGold = (int)Math.Round(earnedGold, MidpointRounding.AwayFromZero);

            if (finalGold > maxGold)
                finalGold = maxGold;

            return finalGold;
        }
    }
}
