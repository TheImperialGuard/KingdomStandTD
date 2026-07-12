using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
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
        private readonly ConfigsProviderService _configProviderService;

        private readonly List<LevelTilePresenter> _levelsTilesPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView screen,
            MainMenuPresentersFactory presentersFactory,
            ViewsFactory viewsFactory,
            LevelsProgressionService levelsProgressionService,
            ConfigsProviderService configProviderService)
        {
            _screen = screen;
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
            _levelsProgressionService = levelsProgressionService;
            _configProviderService = configProviderService;
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
            LevelsListConfig levelsListConfig = _configProviderService.GetConfig<LevelsListConfig>();

            for (int i = 0; i < levelsListConfig.Levels.Count; i++)
            {
                int levelNumber = i + 1;

                if(_levelsProgressionService.CanPlay(levelNumber) == false)
                    break;

                CreateLevelTile(_screen.LevelsPositionsList[i].Position, levelNumber);
            }
        }

        private void CreateLevelTile(RectTransform position, int levelNumber)
        {
            LevelTileView tileView = _viewsFactory.Create<LevelTileView>(ViewIDs.LevelTile, position);

            LevelTilePresenter tilePresenter = _presentersFactory.CreateLevelTilePresenter(tileView, levelNumber);

            _levelsTilesPresenters.Add(tilePresenter);
        }
    }
}
