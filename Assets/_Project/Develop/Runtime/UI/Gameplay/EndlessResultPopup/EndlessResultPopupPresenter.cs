using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.EndlessResultPopup
{
    // Итог забега бесконечного режима вместо окна поражения: достигнутая волна и рекорд или «Новый рекорд!». Забег к этому
    // времени уже засчитан. Временное, чисто функциональное окно: вид по макету — отдельная задача.
    public class EndlessResultPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "Забег окончен";
        private const string WavePrefix = "Волна ";
        private const string RecordPrefix = "Рекорд: волна ";
        private const string NewRecordText = "Новый рекорд!";

        private readonly EndlessResultPopupView _popupView;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly EndlessRunResult _runResult;

        public EndlessResultPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            EndlessResultPopupView popupView,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs gameplayInputArgs,
            EndlessRunResult runResult)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayInputArgs = gameplayInputArgs;
            _runResult = runResult;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            _popupView.SetTitle(TitleName);
            _popupView.SetWave(WavePrefix + _runResult.ReachedWave);
            _popupView.SetRecord(_runResult.IsNewRecord ? NewRecordText : RecordPrefix + _runResult.BestWave);

            _popupView.MenuClicked += OnMenuClicked;
            _popupView.RestartClicked += OnRestartClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            UnsubscribeFromView();
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            UnsubscribeFromView();
        }

        private void UnsubscribeFromView()
        {
            _popupView.MenuClicked -= OnMenuClicked;
            _popupView.RestartClicked -= OnRestartClicked;
        }

        private void OnMenuClicked()
        {
            _coroutinesPerformer.StartPerform(
                _sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, MainMenuInputArgs.FromGameplay(_gameplayInputArgs)));

            OnCloseRequest();
        }

        private void OnRestartClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, _gameplayInputArgs));

            OnCloseRequest();
        }
    }
}
