using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    public class LevelTilePresenter : IPresenter
    {
        private readonly LevelTileView _tileView;

        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private readonly int _levelNumber;

        public LevelTilePresenter(
            LevelTileView tileView, 
            SceneSwitcherService sceneSwitcherService, 
            ICoroutinesPerformer coroutinesPerformer, 
            int levelNumber)
        {
            _tileView = tileView;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _levelNumber = levelNumber;
        }

        public LevelTileView View => _tileView;

        public void Initialize()
        {
            _tileView.Clicked += OnViewClicked;

            _tileView.SetLevel(_levelNumber.ToString());
        }

        public void Dispose()
        {
            _tileView.Clicked -= OnViewClicked;
        }

        private void OnViewClicked()
        {
            _coroutinesPerformer
                .StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(_levelNumber)));
        }
    }
}
