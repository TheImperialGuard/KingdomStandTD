using System;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Settings;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta
{
    public class MainMenuPopupService : PopupService
    {
        private readonly MainMenuUIRoot _uiRoot;
        private readonly MainMenuPresentersFactory _presentersFactory;

        public MainMenuPopupService(
            ViewsFactory viewsFactory,
            MainMenuUIRoot uiRoot,
            MainMenuPresentersFactory mainMenuPresentersFactory)
            : base(viewsFactory)
        {
            _uiRoot = uiRoot;
            _presentersFactory = mainMenuPresentersFactory;
        }

        protected override Transform PopupLayer => _uiRoot.PopupsLayer;

        public PlayBoardPopupPresenter OpenPlayBoardPopup(Action closedCallback = null)
        {
            PlayBoardPopupView view = ViewsFactory.Create<PlayBoardPopupView>(ViewIDs.PlayBoardPopup, PopupLayer);

            PlayBoardPopupPresenter popup = _presentersFactory.CreatePlayBoardPopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public SettingsPopupPresenter OpenSettingsPopup(Action closedCallback = null)
        {
            SettingsPopupView view = ViewsFactory.Create<SettingsPopupView>(ViewIDs.SettingsPopup, PopupLayer);

            SettingsPopupPresenter popup = _presentersFactory.CreateSettingsPopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }
    }
}
