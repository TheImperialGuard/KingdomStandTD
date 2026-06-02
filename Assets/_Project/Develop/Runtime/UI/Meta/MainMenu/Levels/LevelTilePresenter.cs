using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

        private Coroutine _process;

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
            _tileView.Clicked += OnViewClicked;

            _tileView.SetLevel(_levelNumber.ToString());

            if (_levelsProgressionService.IsLevelCompleted(_levelNumber))
            {
                _tileView.SetComplete();
                _tileView.SetResults(_levelsProgressionService.CompletedLevels[_levelNumber]);
            }
            else
            {
                _tileView.SetActive();
            }
        }

        public void Dispose()
        {
            KillProcess();

            _tileView.Clicked -= OnViewClicked;
        }

        public void PlayAnimation(List<Transform> pathPoints)
        {
            KillProcess();

            _process = _coroutinesPerformer.StartPerform(AnimationProcess(pathPoints));
        }

        private IEnumerator AnimationProcess(List<Transform> pathPoints)
        {
            Sequence animation = DOTween.Sequence();

            if (pathPoints.Count > 0)
                AddPathPointsAnimation(animation, pathPoints);

            animation
                .Append(View.Show());

            yield return animation.WaitForCompletion();
        }

        private void AddPathPointsAnimation(Sequence animation, List<Transform> pathPoints)
        {
            foreach (Transform pathPoint in pathPoints)
            {
                pathPoint.localScale.Set(0f, 0f, 0f);

                Tween pointAnimation = pathPoint
                    .DOScale(1, 0.1f)
                    .From(0)
                    .SetUpdate(true)
                    .Play();

                animation.Append(pointAnimation);
            }
        }

        private void OnViewClicked()
        {
            _coroutinesPerformer
                .StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(_levelNumber)));
        }

        private void KillProcess()
        {
            if (_process != null)
                _coroutinesPerformer.StopPerform(_process);
        }
    }
}
