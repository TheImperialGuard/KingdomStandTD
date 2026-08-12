using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet;
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
    }
}
