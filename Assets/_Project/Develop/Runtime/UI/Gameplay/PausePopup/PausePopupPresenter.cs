using Assets._Project.Develop.Runtime.Gameplay.Features.PauseFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.Audio;
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
        private readonly AudioHandler _audioHandler;

        public PausePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            PausePopupView popupView,
            TimeScalePauseService timeScalePauseService,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs gameplayInputArgs,
            AudioHandler audioHandler)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _timeScalePauseService = timeScalePauseService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayInputArgs = gameplayInputArgs;
            _audioHandler = audioHandler;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            _popupView.SetupMusicHandlers(_audioHandler.IsMusicOn, _audioHandler.MusicVolume);
            _popupView.SetupSoundsHandlers(_audioHandler.IsSoundsOn, _audioHandler.SoundsVolume);

            _popupView.ExitClicked += OnExitClicked;
            _popupView.RestartClicked += OnRestartClicked;

            _popupView.MusicVolumeChanged += OnMusicVolumeChanged;
            _popupView.SoundsVolumeChanged += OnSoundsVolumeChanged;

            _popupView.MusicToggleChanged += OnMusicToggleChanged;
            _popupView.SoundsToggleChanged += OnSoundsToggleChanged;

            _timeScalePauseService.Pause();

            _popupView.SetupMusicHandlers(_audioHandler.IsMusicOn, _audioHandler.MusicVolume);
            _popupView.SetupSoundsHandlers(_audioHandler.IsSoundsOn, _audioHandler.SoundsVolume);
        }

        public override void Dispose()
        {
            base.Dispose();

            UnsubscribeFromView();

            _timeScalePauseService.Unpause();
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            UnsubscribeFromView();

            _timeScalePauseService.Unpause();
        }

        private void UnsubscribeFromView()
        {
            _popupView.ExitClicked -= OnExitClicked;
            _popupView.RestartClicked -= OnRestartClicked;

            _popupView.MusicVolumeChanged -= OnMusicVolumeChanged;
            _popupView.SoundsVolumeChanged -= OnSoundsVolumeChanged;

            _popupView.MusicToggleChanged -= OnMusicToggleChanged;
            _popupView.SoundsToggleChanged -= OnSoundsToggleChanged;
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

        private void OnMusicVolumeChanged(float value)
        {
            _audioHandler.SetMusicVolume(value);
        }

        private void OnSoundsVolumeChanged(float value)
        {
            _audioHandler.SetSoundsVolume(value);
        }

        private void OnMusicToggleChanged(bool value)
        {
            if (value == false)
                _audioHandler.OffMusic();
            else
                _audioHandler.OnMusic();
        }

        private void OnSoundsToggleChanged(bool value)
        {
            if (value == false)
                _audioHandler.OffSounds();
            else
                _audioHandler.OnSounds();
        }
    }
}
