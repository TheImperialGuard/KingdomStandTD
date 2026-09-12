using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet;
using Assets._Project.Develop.Runtime.UI.Gameplay.PlayerHealthDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.StagesStatus;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _screenView;

        private readonly GameplayPresentersFactory _gameplayPresentersFactory;

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView screenView, 
            GameplayPresentersFactory gameplayPresentersFactory)
        {
            _screenView = screenView;
            _gameplayPresentersFactory = gameplayPresentersFactory;
        }

        public GameplayScreenView Screen => _screenView;

        public void Initialize()
        {
            CreateGoldWallet();
            CreateStagesStatus();
            CreatePlayerHealth();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {
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
    }
}
