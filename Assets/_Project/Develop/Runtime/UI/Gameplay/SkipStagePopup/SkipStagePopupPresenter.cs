using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup
{
    public class SkipStagePopupPresenter : PopupPresenterBase
    {
        private readonly SkipStagePopupView _view;

        private readonly StagesCycle _stagesCycle;

        private int _clicks;

        private IDisposable _canBeSkipedDisposable;

        public SkipStagePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            SkipStagePopupView view,
            StagesCycle stagesCycle) :
            base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _stagesCycle = stagesCycle;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.StartButtonClicked += OnStartButtonClicked;

            _canBeSkipedDisposable = _stagesCycle.StageCanBeSkiped.Subscribe(OnStageCanBeSkipedChange);

            _clicks = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.StartButtonClicked -= OnStartButtonClicked;
            _canBeSkipedDisposable.Dispose();
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
                _stagesCycle.SkipStage();
                OnCloseRequest();
            }
            else
            {
                _view.ShowInfo();
            }
        }

        private void OnStageCanBeSkipedChange(bool oldValue, bool newValue)
        {
            if (newValue == false)
                OnCloseRequest();
        }
    }
}
