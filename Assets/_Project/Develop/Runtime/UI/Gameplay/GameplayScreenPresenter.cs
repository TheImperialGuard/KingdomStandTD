using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet;
using Assets._Project.Develop.Runtime.UI.Gameplay.PlayerHealthDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.StagesStatus;
using Assets._Project.Develop.Runtime.Utilities.Adverts;
using Assets._Project.Develop.Runtime.Utilities.Wallet;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _screenView;

        private readonly GameplayPresentersFactory _gameplayPresentersFactory;
        private readonly GameplayPopupService _gameplayPopupService;
        private readonly IAdvertsService _advertsService;
        private readonly GameConfig _gameConfig;
        private readonly WalletService _goldWallet;
        private readonly StagesCycle _stagesCycle;

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView screenView,
            GameplayPresentersFactory gameplayPresentersFactory,
            GameplayPopupService gameplayPopupService,
            IAdvertsService advertsService,
            GameConfig gameConfig,
            WalletService goldWallet,
            StagesCycle stagesCycle)
        {
            _screenView = screenView;
            _gameplayPresentersFactory = gameplayPresentersFactory;
            _gameplayPopupService = gameplayPopupService;
            _advertsService = advertsService;
            _gameConfig = gameConfig;
            _goldWallet = goldWallet;
            _stagesCycle = stagesCycle;
        }

        public GameplayScreenView Screen => _screenView;

        public void Initialize()
        {
            _screenView.SetAdvertRewardTipText(_gameConfig.GoldRewardByAdvert.ToString());

            if (_advertsService.IsAvailable == false)
                HideAdvertObjects();

            _screenView.PauseClicked += OnPauseClicked;
            _screenView.AdvertRewardClicked += OnAdvertRewardClicked;
            _stagesCycle.Launched += OnStagesLaunched;
            _advertsService.RewardedAdvertFailed += OnAdvertFailed;

            CreateGoldWallet();
            CreateStagesStatus();
            CreatePlayerHealth();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {
            _screenView.PauseClicked -= OnPauseClicked;
            _screenView.AdvertRewardClicked -= OnAdvertRewardClicked;
            _advertsService.RewardedAdvertFailed -= OnAdvertFailed;
            _stagesCycle.Launched -= OnStagesLaunched;

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _childPresenters.Clear();
        }

        private void CreateGoldWallet()
        {
            GoldWalletPresenter goldWalletPresenter = _gameplayPresentersFactory.CreateGoldWalletPresenter(_screenView.GoldWalletView);

            _childPresenters.Add(goldWalletPresenter);
        }

        private void CreateStagesStatus()
        {
            StagesStatusPresenter stagesStatusPresenter = _gameplayPresentersFactory.CreateStagesStatusPresenter(_screenView.StagesStatusView);

            _childPresenters.Add(stagesStatusPresenter);
        }

        private void CreatePlayerHealth()
        {
            PlayerHealthPresenter playerHealthPresenter = _gameplayPresentersFactory.CreatePlayerHealthPresenter(_screenView.PlayerHealthView);

            _childPresenters.Add(playerHealthPresenter);
        }

        private void OnStagesLaunched()
        {
            _screenView.HideTips();
        }

        private void OnPauseClicked()
        {
            _gameplayPopupService.OpenPausePopup();
        }

        private void OnAdvertRewardClicked()
        {
            _advertsService.OpenRewardedAdvert(GetGoldReward);
        }

        private void OnAdvertFailed()
        {
            // тут в будущем сделать показ попапа с ошибкой, сейчас мало времени
        }

        private void GetGoldReward()
        {
            _goldWallet.Add(CurrencyTypes.Gold, _gameConfig.GoldRewardByAdvert);
            HideAdvertObjects();
        }

        private void HideAdvertObjects()
        {
            _screenView.HideAdvertRewardButton();
            _screenView.HideAdvertRewardTip();
        }
    }
}
