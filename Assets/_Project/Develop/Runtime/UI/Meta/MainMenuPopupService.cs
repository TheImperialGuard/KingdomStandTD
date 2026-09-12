using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
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

        public LevelsMenuPopupPresenter OpenLevelsMenuPopup()
        {
            LevelsMenuPopupView view = ViewsFactory.Create<LevelsMenuPopupView>(ViewIDs.LevelsMenuPopup, PopupLayer);

            LevelsMenuPopupPresenter popup = _presentersFactory.CreateLevelsMenuPopupPresenter(view);

            OnPopupCreated(popup, view);

            return popup;
        }
    }
}
