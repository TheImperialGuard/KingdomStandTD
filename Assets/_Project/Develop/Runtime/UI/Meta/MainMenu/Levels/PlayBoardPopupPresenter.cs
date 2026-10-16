using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    public class PlayBoardPopupPresenter : PopupPresenterBase
    {
        public const string TitleName = "Уровни";

        private readonly ConfigsProviderService _configProviderService;
        private readonly LevelsProgressionService _levelsProgressionService;

        private readonly MainMenuPresentersFactory _presentersFactory;
        private readonly ViewsFactory _viewsFactory;

        private readonly PlayBoardPopupView _boardView;

        private readonly List<LevelTilePresenter> _levelTilePresenters = new();

        public PlayBoardPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService, 
            MainMenuPresentersFactory presentersFactory,
            ViewsFactory viewsFactory,
            LevelsProgressionService levelsProgressionService,
            ConfigsProviderService configProviderService,
            PlayBoardPopupView boardView) : base(coroutinesPerformer, rayShooterService)
        {
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
            _levelsProgressionService = levelsProgressionService;
            _configProviderService = configProviderService;
            _boardView = boardView;
        }

        public override void Initialize()
        {
            base.Initialize();

            _boardView.SetTitle(TitleName);

            LevelsListConfig levelsListConfig = _configProviderService.GetConfig<LevelsListConfig>();

            for (int i = 0; i < levelsListConfig.Levels.Count; i++)
            {
                int levelNumber = i + 1;

                if (_levelsProgressionService.CanPlay(levelNumber) == false)
                    break;

                CreateLevelTile(levelNumber);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (LevelTilePresenter levelTilePresenter in _levelTilePresenters)
            {
                _boardView.LevelTilesListView.RemoveElement(levelTilePresenter.View);
                _viewsFactory.Release(levelTilePresenter.View);
                levelTilePresenter.Dispose();
            }

            _levelTilePresenters.Clear();
        }

        protected override PopupViewBase PopupView => _boardView;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            foreach (LevelTilePresenter levelTilePresenter in _levelTilePresenters)
                levelTilePresenter.Subscribe();
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            foreach (LevelTilePresenter levelTilePresenter in _levelTilePresenters)
                levelTilePresenter.Unsubscribe();
        }

        private void CreateLevelTile(int levelNumber)
        {
            LevelTileView levelTileView = _viewsFactory.Create<LevelTileView>(ViewIDs.LevelTile);

            _boardView.LevelTilesListView.AddElement(levelTileView);

            LevelTilePresenter levelTilePresenter = _presentersFactory.CreateLevelTilePresenter(levelTileView, levelNumber);

            levelTilePresenter.Initialize();

            _levelTilePresenters.Add(levelTilePresenter);
        }
    }
}
