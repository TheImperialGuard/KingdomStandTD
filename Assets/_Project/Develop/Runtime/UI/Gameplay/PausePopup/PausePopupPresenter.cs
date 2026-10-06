using Assets._Project.Develop.Runtime.Gameplay.Features.PauseFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Settings;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.PausePopup
{
    public class PausePopupPresenter : PopupPresenterBase
    {
        private readonly PausePopupView _popupView;
        private readonly TimeScalePauseService _timeScalePauseService;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly GameplayPresentersFactory _presentersFactory;

        private AudioSettingsPresenter _audioSettingsPresenter;

        public PausePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            PausePopupView popupView,
            TimeScalePauseService timeScalePauseService,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs gameplayInputArgs,
            GameplayPresentersFactory presentersFactory)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _timeScalePauseService = timeScalePauseService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayInputArgs = gameplayInputArgs;
            _presentersFactory = presentersFactory;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            CreateAudioSettings();

            _popupView.ExitClicked += OnExitClicked;
            _popupView.RestartClicked += OnRestartClicked;

            _timeScalePauseService.Pause();
        }

        public override void Dispose()
        {
            base.Dispose();

            UnsubscribeFromView();

            _audioSettingsPresenter.Dispose();

            _timeScalePauseService.Unpause();
        }

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _audioSettingsPresenter.Subscribe();
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            UnsubscribeFromView();

            _audioSettingsPresenter.Unsubscribe();

            _timeScalePauseService.Unpause();
        }

        private void CreateAudioSettings()
        {
            _audioSettingsPresenter = _presentersFactory.CreateAudioSettingsPresenter(_popupView.AudioSettingsView);

            _audioSettingsPresenter.Initialize();
        }

        private void UnsubscribeFromView()
        {
            _popupView.ExitClicked -= OnExitClicked;
            _popupView.RestartClicked -= OnRestartClicked;
        }

        private void OnExitClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs(Scenes.Gameplay)));
            OnCloseRequest();
        }

        private void OnRestartClicked()
        {
            _coroutinesPerformer.StartPerform(
                _sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(_gameplayInputArgs.LevelNumber)));
            OnCloseRequest();
        }
    }
}
