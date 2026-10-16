using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Text;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    // Доска «Играть»: шаг «Режим игры» и шаг «Кампания» со всеми уровнями и постоянной плиткой «В разработке» (Р-07).
    public class PlayBoardPopupPresenter : PopupPresenterBase, IMainMenuPanel
    {
        private const string LevelWordOne = "уровень";
        private const string LevelWordFew = "уровня";
        private const string LevelWordMany = "уровней";

        private readonly ConfigsProviderService _configProviderService;
        private readonly LevelsProgressionService _levelsProgressionService;

        private readonly MainMenuPresentersFactory _presentersFactory;
        private readonly ViewsFactory _viewsFactory;

        private readonly PlayBoardPopupView _boardView;
        private readonly PlayBoardSteps _startStep;

        private readonly List<LevelTilePresenter> _levelTilePresenters = new();

        private LevelTileView _inDevelopmentTile;
        private int _focusedTileIndex;

        public PlayBoardPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            MainMenuPresentersFactory presentersFactory,
            ViewsFactory viewsFactory,
            LevelsProgressionService levelsProgressionService,
            ConfigsProviderService configProviderService,
            PlayBoardPopupView boardView,
            PlayBoardSteps startStep) : base(coroutinesPerformer, rayShooterService)
        {
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
            _levelsProgressionService = levelsProgressionService;
            _configProviderService = configProviderService;
            _boardView = boardView;
            _startStep = startStep;
        }

        protected override PopupViewBase PopupView => _boardView;

        public override void Initialize()
        {
            base.Initialize();

            LevelsListConfig levelsListConfig = _configProviderService.GetConfig<LevelsListConfig>();
            int levelsCount = levelsListConfig.Levels.Count;

            for (int i = 0; i < levelsCount; i++)
                CreateLevelTile(i + 1);

            CreateInDevelopmentTile();

            string levelWord = RussianPlural.Select(levelsCount, LevelWordOne, LevelWordFew, LevelWordMany);
            _boardView.SetCampaignSubtitle($"{levelsCount} {levelWord}");

            _focusedTileIndex = GetFocusedTileIndex(levelsCount);
            _boardView.RefreshList();

            ShowStep(_startStep);
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (LevelTilePresenter levelTilePresenter in _levelTilePresenters)
            {
                ReleaseTile(levelTilePresenter.View);
                levelTilePresenter.Dispose();
            }

            _levelTilePresenters.Clear();

            ReleaseTile(_inDevelopmentTile);
            _inDevelopmentTile = null;
        }

        public void ApplyLayout(MainMenuLayout layout)
        {
            _boardView.SetLayout(layout.PanelWidth, layout.PanelTop, layout.PanelRightMargin);
            _boardView.ScrollToTile(_focusedTileIndex);
        }

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _boardView.CampaignClicked += OnCampaignClicked;
            _boardView.BackClicked += OnBackClicked;

            foreach (LevelTilePresenter levelTilePresenter in _levelTilePresenters)
                levelTilePresenter.Subscribe();
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _boardView.CampaignClicked -= OnCampaignClicked;
            _boardView.BackClicked -= OnBackClicked;

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

        // Плитка без логики и без презентера: не нажимается.
        private void CreateInDevelopmentTile()
        {
            _inDevelopmentTile = _viewsFactory.Create<LevelTileView>(ViewIDs.LevelTile);
            _boardView.LevelTilesListView.AddElement(_inDevelopmentTile);
            _inDevelopmentTile.SetInDevelopment();
        }

        private void ReleaseTile(LevelTileView tile)
        {
            _boardView.LevelTilesListView.RemoveElement(tile);
            _viewsFactory.Release(tile);
        }

        // К ряду текущего уровня, а если пройдено всё — к последнему ряду, где «В разработке» (Р-56).
        private int GetFocusedTileIndex(int levelsCount)
        {
            for (int i = 0; i < levelsCount; i++)
            {
                int levelNumber = i + 1;

                if (_levelsProgressionService.IsLevelCompleted(levelNumber) == false)
                    return i;
            }

            return levelsCount;
        }

        private void ShowStep(PlayBoardSteps step)
        {
            _boardView.ShowStep(step);

            if (step == PlayBoardSteps.Campaign)
                _boardView.ScrollToTile(_focusedTileIndex);
        }

        private void OnCampaignClicked() => ShowStep(PlayBoardSteps.Campaign);

        private void OnBackClicked() => ShowStep(PlayBoardSteps.Modes);
    }
}
