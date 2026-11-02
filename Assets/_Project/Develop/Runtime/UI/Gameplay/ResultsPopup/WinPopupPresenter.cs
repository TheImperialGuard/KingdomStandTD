using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup
{
    public class WinPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "Победа!";
        private const string DecreeText = "Корона благодарит вас за службу";
        private const string LivesSeparator = "/";

        private readonly WinPopupView _popupView;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly LevelResults _levelResults;
        private readonly PlayerHealth _playerHealth;

        public WinPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            WinPopupView popupView,
            SceneSwitcherService sceneSwitcherService,
            LevelResults levelResults,
            PlayerHealth playerHealth)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _levelResults = levelResults;
            _playerHealth = playerHealth;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            _popupView.SetTitle(TitleName);
            _popupView.SetDecree(DecreeText);
            _popupView.SetResults(_levelResults);
            _popupView.SetLives(_playerHealth.Current.Value + LivesSeparator + _playerHealth.Max.Value);

            _popupView.ContinueClicked += OnContinueClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _popupView.ContinueClicked -= OnContinueClicked;
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _popupView.ContinueClicked -= OnContinueClicked;
        }

        private void OnContinueClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs(Scenes.Gameplay)));
            OnCloseRequest();
        }
    }
}
