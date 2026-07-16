using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPopupService : PopupService
    {
        private readonly GameplayUIRoot _uIRoot;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;

        public GameplayPopupService(
            ViewsFactory viewsFactory,
            GameplayUIRoot uIRoot,
            GameplayPresentersFactory gameplayPresentersFactory)
            : base(viewsFactory)
        {
            _uIRoot = uIRoot;
            _gameplayPresentersFactory = gameplayPresentersFactory;
        }

        protected override Transform PopupLayer => _uIRoot.PopupsLayer;

        public BuildTowerPopupPresenter OpenBuildTowerPopup(Entity towerPlaceholder, Action closedCallback = null)
        {
            BuildTowerPopupView view = ViewsFactory.Create<BuildTowerPopupView>(ViewIDs.BuildTowerPopup, PopupLayer);

            BuildTowerPopupPresenter popup = _gameplayPresentersFactory.CreateBuildTowerPopupPresenter(view, towerPlaceholder);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }
    }
}
