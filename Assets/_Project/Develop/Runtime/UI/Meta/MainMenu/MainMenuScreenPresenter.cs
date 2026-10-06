using Assets._Project.Develop.Runtime.UI.Core.Presenters;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _view;
        private readonly MainMenuPopupService _popupService;

        public MainMenuScreenPresenter(MainMenuScreenView view, MainMenuPopupService popupService)
        {
            _view = view;
            _popupService = popupService;
        }

        public void Initialize()
        {
            _view.OpenLevelsMenuButtonClicked += OnOpenLevelsMenuButtonClicked;
            _view.OpenSettingsButtonClicked += OnOpenSettingsButtonClicked;
        }

        public void Dispose()
        {
            _view.OpenLevelsMenuButtonClicked -= OnOpenLevelsMenuButtonClicked;
            _view.OpenSettingsButtonClicked -= OnOpenSettingsButtonClicked;
        }

        private void OnOpenLevelsMenuButtonClicked()
        {
            _popupService.OpenLevelsMenuPopup();
        }

        private void OnOpenSettingsButtonClicked()
        {
            _popupService.OpenSettingsPopup();
        }
    }
}
