using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _screen;

        private readonly MainMenuPresentersFactory _presentersFactory;
        private readonly ViewsFactory _viewsFactory;

        private readonly LevelsProgressionService _levelsProgressionService;

        private readonly List<LevelTilePresenter> _levelsTilesPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView screen,
            MainMenuPresentersFactory presentersFactory,
            ViewsFactory viewsFactory,
            LevelsProgressionService levelsProgressionService)
        {
            _screen = screen;
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
            _levelsProgressionService = levelsProgressionService;
        }

        public void Initialize()
        {
            CreateLevelsList();

            foreach (LevelTilePresenter presenter in _levelsTilesPresenters)
            {
                presenter.Initialize();
            }
        }

        public void ShowLastLevelAnimation()
        {
            int lastLevelNumber = _levelsTilesPresenters.Count;

            _levelsTilesPresenters[lastLevelNumber - 1]
                .PlayAnimation(_screen.LevelsPositionsList[lastLevelNumber - 1].PathPointViews);
        }

        public void Dispose()
        {
            foreach (LevelTilePresenter presenter in _levelsTilesPresenters)
            {
                _viewsFactory.Release(presenter.View);
                presenter.Dispose();
            }
        }

        private void CreateLevelsList()
        {
            for (int i = 1; _levelsProgressionService.CanPlay(i); i++)
                CreateLevelTile(_screen.LevelsPositionsList[i - 1].Position, i);
        }

        private void CreateLevelTile(RectTransform position, int levelNumber)
        {
            LevelTileView tileView = _viewsFactory.Create<LevelTileView>(ViewIDs.LevelTile, position);

            LevelTilePresenter tilePresenter = _presentersFactory.CreateLevelTilePresenter(tileView, levelNumber);

            _levelsTilesPresenters.Add(tilePresenter);
        }
    }
}
