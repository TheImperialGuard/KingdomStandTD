using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Endless
{
    // Окно бесконечного режима перед стартом, поверх доски «Играть»: описание режима из его конфига, «Играть» — в бой,
    // «Назад» — обратно к доске. Временное, чисто функциональное окно: вид по макету — отдельная задача.
    public class EndlessModePopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "Бесконечный режим";

        private readonly EndlessModePopupView _popupView;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly EndlessModeConfig _config;

        public EndlessModePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            EndlessModePopupView popupView,
            SceneSwitcherService sceneSwitcherService,
            EndlessModeConfig config)
            : base(coroutinesPerformer, rayShooterService)
        {
            _popupView = popupView;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _config = config;
        }

        protected override PopupViewBase PopupView => _popupView;

        public override void Initialize()
        {
            base.Initialize();

            _popupView.SetTitle(TitleName);
            _popupView.SetDescription(_config.Description);

            _popupView.PlayClicked += OnPlayClicked;
            _popupView.BackClicked += OnBackClicked;
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
            _popupView.PlayClicked -= OnPlayClicked;
            _popupView.BackClicked -= OnBackClicked;
        }

        private void OnPlayClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, GameplayInputArgs.ForEndless()));

            OnCloseRequest();
        }

        private void OnBackClicked() => OnCloseRequest();
    }
}
