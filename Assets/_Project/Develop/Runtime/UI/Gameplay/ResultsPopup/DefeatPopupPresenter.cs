using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup
{
    public class DefeatPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "Поражение";

        private readonly DefeatPopupView _popupView;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayInputArgs _gameplayInputArgs;

        public DefeatPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            DefeatPopupView popupView,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs gameplayInputArgs)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayInputArgs = gameplayInputArgs;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            _popupView.SetTitle(TitleName);

            _popupView.ContinueClicked += OnContinueClicked;
            _popupView.RestartClicked += OnRestartClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _popupView.ContinueClicked -= OnContinueClicked;
            _popupView.RestartClicked -= OnRestartClicked;
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _popupView.ContinueClicked -= OnContinueClicked;
            _popupView.RestartClicked -= OnRestartClicked;
        }

        private void OnContinueClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs()));
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
