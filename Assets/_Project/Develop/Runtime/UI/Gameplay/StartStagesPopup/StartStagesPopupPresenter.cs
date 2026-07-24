using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup
{
    public class StartStagesPopupPresenter : PopupPresenterBase
    {
        private readonly StartStagesPopupView _view;

        private readonly StagesCycle _stagesCycle;

        private int _clicks;

        public StartStagesPopupPresenter(
            StartStagesPopupView view,
            ICoroutinesPerformer coroutinesPerformer,
            StagesCycle stagesCycle,
            RayShooterService rayShooterService)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _stagesCycle = stagesCycle;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.StartButtonClicked += OnStartButtonClicked;

            _clicks = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.StartButtonClicked -= OnStartButtonClicked;
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            _view.HideInfo();
            _clicks = 0;
        }

        private void OnStartButtonClicked()
        {
            if (_clicks++ > 0)
            {
                _stagesCycle.Launch();
                OnCloseRequest();
            }
            else
            {
                _view.ShowInfo();
            }
        }
    }
}
