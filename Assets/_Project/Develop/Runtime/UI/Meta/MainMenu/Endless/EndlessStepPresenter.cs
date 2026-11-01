using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Features.Endless;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Endless
{
    // Шаг «Бесконечный режим» доски «Играть»: описание из конфига режима, рекорд, «Играть» — в бой. Подписки включает и
    // выключает доска вместе со своими.
    public class EndlessStepPresenter : IPresenter
    {
        // После двоеточия — зазор 10 px макета (15 u): пробел Dela на этом кегле вдвое уже.
        private const string RecordPrefix = "Рекорд:<space=15>волна ";
        private const string NoRecordText = "Рекорда пока нет";

        private readonly EndlessStepView _stepView;

        private readonly EndlessModeConfig _config;
        private readonly EndlessRecordService _endlessRecordService;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public EndlessStepPresenter(
            EndlessStepView stepView,
            EndlessModeConfig config,
            EndlessRecordService endlessRecordService,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _stepView = stepView;
            _config = config;
            _endlessRecordService = endlessRecordService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void Initialize()
        {
            _stepView.SetDescription(_config.Goal, _config.Advice);
            _stepView.SetRecord(GetRecordText());
        }

        public void Dispose()
        {
            _stepView.PlayClicked -= OnPlayClicked;
        }

        public void Subscribe()
        {
            _stepView.PlayClicked += OnPlayClicked;
        }

        public void Unsubscribe()
        {
            _stepView.PlayClicked -= OnPlayClicked;
        }

        private string GetRecordText()
        {
            if (_endlessRecordService.HasRecord)
                return RecordPrefix + _endlessRecordService.BestWave;

            return NoRecordText;
        }

        private void OnPlayClicked()
        {
            _coroutinesPerformer
                .StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, GameplayInputArgs.ForEndless()));
        }
    }
}
