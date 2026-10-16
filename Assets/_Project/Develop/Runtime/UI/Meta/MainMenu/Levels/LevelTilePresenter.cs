using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    public class LevelTilePresenter : IPresenter
    {
        private readonly LevelTileView _tileView;

        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly LevelsProgressionService _levelsProgressionService;

        private readonly int _levelNumber;

        public LevelTilePresenter(
            LevelTileView tileView,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer,
            int levelNumber,
            LevelsProgressionService levelsProgressionService)
        {
            _tileView = tileView;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _levelNumber = levelNumber;
            _levelsProgressionService = levelsProgressionService;
        }

        public LevelTileView View => _tileView;

        public void Initialize()
        {
            _tileView.SetNumber(_levelNumber);

            if (_levelsProgressionService.IsLevelCompleted(_levelNumber))
                _tileView.SetCompleted(_levelsProgressionService.CompletedLevels[_levelNumber]);
            else if (_levelsProgressionService.CanPlay(_levelNumber))
                _tileView.SetCurrent();
            else
                _tileView.SetClosed();
        }

        public void Dispose()
        {
            _tileView.Clicked -= OnViewClicked;
        }

        public void Subscribe()
        {
            _tileView.Clicked += OnViewClicked;
        }

        public void Unsubscribe()
        {
            _tileView.Clicked -= OnViewClicked;
        }

        private void OnViewClicked()
        {
            if (_levelsProgressionService.CanPlay(_levelNumber) == false)
            {
                Debug.Log("Уровень заблокирован, пройдите предыдущий");
                return;
            }

            _coroutinesPerformer
                .StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(_levelNumber)));
        }
    }
}
