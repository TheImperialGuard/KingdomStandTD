using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.PauseFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.PausePopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerAbilitiesPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup;
using Assets._Project.Develop.Runtime.UI.ProjectContext;
using Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPopupService : ProjectPopupService
    {
        private readonly GameplayUIRoot _uIRoot;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;
        private readonly TimeScalePauseService _timeScalePauseService;

        public GameplayPopupService(
            ViewsFactory viewsFactory,
            ProjectPresentersFactory projectPresentersFactory,
            GameplayUIRoot uIRoot,
            GameplayPresentersFactory gameplayPresentersFactory,
            TimeScalePauseService timeScalePauseService)
            : base(viewsFactory, projectPresentersFactory)
        {
            _uIRoot = uIRoot;
            _gameplayPresentersFactory = gameplayPresentersFactory;
            _timeScalePauseService = timeScalePauseService;
        }

        protected override Transform PopupLayer => _uIRoot.PopupsLayer;

        protected override EncyclopediaModes EncyclopediaMode => EncyclopediaModes.Standalone;

        protected override void OnEncyclopediaOpened() => _timeScalePauseService.Pause();

        protected override void OnEncyclopediaClosed() => _timeScalePauseService.Unpause();

        public StartStagesPopupPresenter OpenStartStagesPopup(Action closedCallback = null)
        {
            StartStagesPopupView view = ViewsFactory.Create<StartStagesPopupView>(ViewIDs.StartStagesPopup, PopupLayer);

            StartStagesPopupPresenter popup = _gameplayPresentersFactory.CreateStartStagesPopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public SkipStagePopupPresenter OpenSkipStagePopup(Action closedCallback = null)
        {
            SkipStagePopupView view = ViewsFactory.Create<SkipStagePopupView>(ViewIDs.SkipStagePopup, PopupLayer);

            SkipStagePopupPresenter popup = _gameplayPresentersFactory.CreateSkipStagePopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public BuildTowerPopupPresenter OpenBuildTowerPopup(Entity towerPlaceholder, Action closedCallback = null)
        {
            BuildTowerPopupView view = ViewsFactory.Create<BuildTowerPopupView>(ViewIDs.BuildTowerPopup, PopupLayer);

            BuildTowerPopupPresenter popup = _gameplayPresentersFactory.CreateBuildTowerPopupPresenter(view, towerPlaceholder);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public UpgradeTowerPopupPresenter OpenUpgradeTowerPopup(Entity sourceTower, Action closedCallback = null)
        {
            UpgradeTowerPopupView view = ViewsFactory.Create<UpgradeTowerPopupView>(ViewIDs.UpgradeTowerPopup, PopupLayer);

            UpgradeTowerPopupPresenter popup = _gameplayPresentersFactory.CreateUpgradeTowerPopupPresenter(view, sourceTower);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public TowerAbilitiesPopupPresenter OpenTowerAbilitiesPopup(Entity sourceTower, Action closedCallback = null)
        {
            TowerAbilitiesPopupView view = ViewsFactory.Create<TowerAbilitiesPopupView>(ViewIDs.TowerAbilitiesPopup, PopupLayer);

            TowerAbilitiesPopupPresenter popup = _gameplayPresentersFactory.CreateTowerAbilitiesPopupPresenter(view, sourceTower);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public TowerInfoPopupPresenter OpenTowerInfoPopup(Entity sourceTower, Action closedCallback = null)
        {
            BottomInfoPopupView view = ViewsFactory.Create<BottomInfoPopupView>(ViewIDs.BottomInfoPopup, PopupLayer);

            TowerInfoPopupPresenter popup = _gameplayPresentersFactory.CreateTowerInfoPopupPresenter(view, sourceTower);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public EnemyInfoPopupPresenter OpenEnemyInfoPopup(Entity sourceTower, Action closedCallback = null)
        {
            BottomInfoPopupView view = ViewsFactory.Create<BottomInfoPopupView>(ViewIDs.BottomInfoPopup, PopupLayer);

            EnemyInfoPopupPresenter popup = _gameplayPresentersFactory.CreateEnemyInfoPopupPresenter(view, sourceTower);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public WinPopupPresenter OpenWinPopup(LevelResults results, Action closedCallback = null)
        {
            WinPopupView view = ViewsFactory.Create<WinPopupView>(ViewIDs.WinPopup, PopupLayer);

            WinPopupPresenter presenter = _gameplayPresentersFactory.CreateWinPopupPresenter(view, results);

            OnPopupCreated(presenter, view, closedCallback);

            return presenter;
        }

        public DefeatPopupPresenter OpenDefeatPopup(Action closedCallback = null)
        {
            DefeatPopupView view = ViewsFactory.Create<DefeatPopupView>(ViewIDs.DefeatPopup, PopupLayer);

            DefeatPopupPresenter presenter = _gameplayPresentersFactory.CreateDefeatPopupPresenter(view);

            OnPopupCreated(presenter, view, closedCallback);

            return presenter;
        }

        public PausePopupPresenter OpenPausePopup(Action closedCallback = null)
        {
            PausePopupView view = ViewsFactory.Create<PausePopupView>(ViewIDs.PausePopup, PopupLayer);

            PausePopupPresenter presenter = _gameplayPresentersFactory.CreatePausePopupPresenter(view);

            OnPopupCreated(presenter, view, closedCallback);

            return presenter;
        }
    }
}
