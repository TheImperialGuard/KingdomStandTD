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

        private readonly List<LevelTilePresenter> _levelsTilesPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView screen, 
            MainMenuPresentersFactory presentersFactory, 
            ViewsFactory viewsFactory)
        {
            _screen = screen;
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
        }

        public void Initialize()
        {
            CreateLevelsList();

            foreach (LevelTilePresenter presenter in _levelsTilesPresenters)
            {
                presenter.Initialize();
            }
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
            int index = 1;

            foreach (RectTransform position in _screen.LevelsPositionsList)
            {
                LevelTileView tileView = _viewsFactory.Create<LevelTileView>(ViewIDs.LevelTile, position);

                LevelTilePresenter tilePresenter = _presentersFactory.CreateLevelTilePresenter(tileView, index);

                _levelsTilesPresenters.Add(tilePresenter);

                index++;
            }
        }
    }
}
