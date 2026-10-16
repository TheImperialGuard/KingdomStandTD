using System;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Settings;
using Assets._Project.Develop.Runtime.UI.ProjectContext;
using Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta
{
    public class MainMenuPopupService : ProjectPopupService
    {
        private readonly MainMenuUIRoot _uiRoot;
        private readonly MainMenuPresentersFactory _presentersFactory;

        public MainMenuPopupService(
            ViewsFactory viewsFactory,
            ProjectPresentersFactory projectPresentersFactory,
            MainMenuUIRoot uiRoot,
            MainMenuPresentersFactory mainMenuPresentersFactory)
            : base(viewsFactory, projectPresentersFactory)
        {
            _uiRoot = uiRoot;
            _presentersFactory = mainMenuPresentersFactory;
        }

        protected override Transform PopupLayer => _uiRoot.PopupsLayer;

        protected override EncyclopediaModes EncyclopediaMode => EncyclopediaModes.Menu;

        public PlayBoardPopupPresenter OpenPlayBoardPopup(PlayBoardSteps startStep, Action closedCallback = null)
        {
            PlayBoardPopupView view = ViewsFactory.Create<PlayBoardPopupView>(ViewIDs.PlayBoardPopup, PopupLayer);

            PlayBoardPopupPresenter popup = _presentersFactory.CreatePlayBoardPopupPresenter(view, startStep);

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
