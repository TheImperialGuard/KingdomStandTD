using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.PauseFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
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
        private readonly HotkeysService _hotkeysService;

        private AudioSettingsPresenter _audioSettingsPresenter;
        private IDisposable _closeRequestedDisposable;

        public PausePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            PausePopupView popupView,
            TimeScalePauseService timeScalePauseService,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs gameplayInputArgs,
            GameplayPresentersFactory presentersFactory,
            HotkeysService hotkeysService)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _timeScalePauseService = timeScalePauseService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayInputArgs = gameplayInputArgs;
            _presentersFactory = presentersFactory;
            _hotkeysService = hotkeysService;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            CreateAudioSettings();

            _popupView.ContinueClicked += OnContinueClicked;
            _popupView.ExitClicked += OnExitClicked;
            _popupView.RestartClicked += OnRestartClicked;

            _timeScalePauseService.Pause();
        }

        public override void Dispose()
        {
            base.Dispose();

            UnsubscribeFromInput();

            _audioSettingsPresenter.Dispose();

            _timeScalePauseService.Unpause();
        }

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _audioSettingsPresenter.Subscribe();

            // Esc закрывает паузу на компьютере, как «Продолжить» (Р-15).
            _closeRequestedDisposable = _hotkeysService.CloseRequested.Subscribe(OnContinueClicked);
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            UnsubscribeFromInput();

            _audioSettingsPresenter.Unsubscribe();

            _timeScalePauseService.Unpause();
        }

        private void CreateAudioSettings()
        {
            _audioSettingsPresenter = _presentersFactory.CreateAudioSettingsPresenter(_popupView.AudioSettingsView);

            _audioSettingsPresenter.Initialize();
        }

        private void UnsubscribeFromInput()
        {
            _popupView.ContinueClicked -= OnContinueClicked;
            _popupView.ExitClicked -= OnExitClicked;
            _popupView.RestartClicked -= OnRestartClicked;

            _closeRequestedDisposable?.Dispose();
            _closeRequestedDisposable = null;
        }

        private void OnContinueClicked() => OnCloseRequest();

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
