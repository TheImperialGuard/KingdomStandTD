using Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup
{
    public class SkipStagePopupPresenter : PopupPresenterBase
    {
        private readonly SkipStagePopupView _view;

        private readonly StagesCycle _stagesCycle;
        private readonly StageProviderService _stageProviderService;
        private readonly EarnGoldOnSkipStageService _earnGoldOnSkipStageService;

        private int _clicks;
        private int _earnedGold;

        private float _lifetime;

        private IDisposable _canBeSkipedDisposable;
        private IDisposable _remainingTimeDisposable;

        public SkipStagePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            SkipStagePopupView view,
            StagesCycle stagesCycle,
            StageProviderService stageProviderService,
            EarnGoldOnSkipStageService earnGoldOnSkipStageService) :
            base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _stagesCycle = stagesCycle;
            _stageProviderService = stageProviderService;
            _earnGoldOnSkipStageService = earnGoldOnSkipStageService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.StartButtonClicked += OnStartButtonClicked;

            _canBeSkipedDisposable = _stagesCycle.StageCanBeSkiped.Subscribe(OnStageCanBeSkipedChange);
            _view.SetWavesEnemiesText(StageEnemiesText.Build(_stageProviderService.NextStageConfig));

            IReadOnlyVariable<float> remainingTime = _stageProviderService.CurrentStageRemainingTime;

            _lifetime = remainingTime.Value;
            _view.SetLifetimeProgress(0f);

            _earnedGold = _earnGoldOnSkipStageService.CalculateEarnedGold(_lifetime);
            _view.SetEarnedGold(_earnedGold);

            _remainingTimeDisposable = remainingTime.Subscribe(OnRemainingTimeChanged);

            _clicks = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.StartButtonClicked -= OnStartButtonClicked;
            _canBeSkipedDisposable.Dispose();
            _remainingTimeDisposable.Dispose();
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

        private void OnRemainingTimeChanged(float oldValue, float newValue)
        {
            float progress = Mathf.InverseLerp(_lifetime, 0f, newValue);

            _view.SetLifetimeProgress(progress);

            UpdateEarnedGold(newValue);
        }

        private void UpdateEarnedGold(float remainingTime)
        {
            int earnedGold = _earnGoldOnSkipStageService.CalculateEarnedGold(remainingTime);

            if (earnedGold == _earnedGold)
                return;

            _earnedGold = earnedGold;
            _view.SetEarnedGold(earnedGold);
        }
    }
}
