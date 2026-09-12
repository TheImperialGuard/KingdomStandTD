using Assets._Project.Develop.Runtime.UI.Core.Presenters;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        MainMenuScreenView _view;
        MainMenuPopupService _popupService;

        public MainMenuScreenPresenter(MainMenuScreenView view, MainMenuPopupService popupService)
        {
            _view = view;
            _popupService = popupService;
        }

        public void Initialize()
        {
            _view.OpenLevelsMenuButtonClicked += OnOpenLevelsMenuButtonClicked;
        }

        public void Dispose()
        {
            _view.OpenLevelsMenuButtonClicked -= OnOpenLevelsMenuButtonClicked;
        }

        private void OnOpenLevelsMenuButtonClicked()
        {
            _popupService.OpenLevelsMenuPopup();
        }
    }
}
