using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup;
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

        public StartStagesPopupPresenter OpenStartStagesPopup(Action closedCallback = null)
        {
            StartStagesPopupView view = ViewsFactory.Create<StartStagesPopupView>(ViewIDs.StartStagesPopup, PopupLayer);

            StartStagesPopupPresenter popup = _gameplayPresentersFactory.CreateStartStagesPopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public SkipStagePopupPresenter OpenSkipStagePopup(Action closedCallback = null)
        {
            SkipStagePopupView view = ViewsFactory.Create<SkipStagePopupView>(ViewIDs.SkipStagePopup, PopupLayer);

            SkipStagePopupPresenter popup = _gameplayPresentersFactory.CreateSkipStagePopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public BuildTowerPopupPresenter OpenBuildTowerPopup(Entity towerPlaceholder, Action closedCallback = null)
        {
            BuildTowerPopupView view = ViewsFactory.Create<BuildTowerPopupView>(ViewIDs.BuildTowerPopup, PopupLayer);

            BuildTowerPopupPresenter popup = _gameplayPresentersFactory.CreateBuildTowerPopupPresenter(view, towerPlaceholder);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public UpgradeTowerPopupPresenter OpenUpgradeTowerPopup(Entity sourceTower, Action closedCallback = null)
        {
            UpgradeTowerPopupView view = ViewsFactory.Create<UpgradeTowerPopupView>(ViewIDs.UpgradeTowerPopup, PopupLayer);

            UpgradeTowerPopupPresenter popup = _gameplayPresentersFactory.CreateUpgradeTowerPopupPresenter(view, sourceTower);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }
    }
}
