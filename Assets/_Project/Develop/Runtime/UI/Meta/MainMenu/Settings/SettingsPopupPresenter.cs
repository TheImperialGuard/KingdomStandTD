using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Settings;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Settings
{
    public class SettingsPopupPresenter : PopupPresenterBase
    {
        private readonly SettingsPopupView _popupView;
        private readonly MainMenuPresentersFactory _presentersFactory;

        private AudioSettingsPresenter _audioSettingsPresenter;

        public SettingsPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            SettingsPopupView popupView,
            MainMenuPresentersFactory presentersFactory)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _presentersFactory = presentersFactory;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            CreateAudioSettings();
        }

        public override void Dispose()
        {
            base.Dispose();

            _audioSettingsPresenter.Dispose();
        }

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _audioSettingsPresenter.Subscribe();
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _audioSettingsPresenter.Unsubscribe();
        }

        private void CreateAudioSettings()
        {
            _audioSettingsPresenter = _presentersFactory.CreateAudioSettingsPresenter(_popupView.AudioSettingsView);

            _audioSettingsPresenter.Initialize();
        }
    }
}
