using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public class StagesCycle : IDisposable
    {
        public event Action Launched;

        private readonly StageProviderService _stageProviderService;
        private readonly GameplayPopupService _gameplayPopupService;
        private readonly EarnGoldOnSkipStageService _earnGoldOnSkipStageService;

        private ReactiveVariable<bool> _isCycleComplete = new();
        private ReactiveVariable<bool> _stageCanBeSkiped = new();

        private bool _isRunning;

        private List<IDisposable> _disposables = new();

        public StagesCycle(
            StageProviderService stageProviderService,
            GameplayPopupService gameplayPopupService,
            EarnGoldOnSkipStageService earnGoldOnSkipStageService)
        {
            _stageProviderService = stageProviderService;

            _disposables.Add(_stageProviderService.CurrentStageResult.Subscribe(OnStageResultChanged));
            _gameplayPopupService = gameplayPopupService;
            _earnGoldOnSkipStageService = earnGoldOnSkipStageService;
        }

        public IReadOnlyVariable<bool> IsCycleComplete => _isCycleComplete;
        public IReadOnlyVariable<bool> StageCanBeSkiped => _stageCanBeSkiped;

        public bool InLastStage => _stageProviderService.HasNextStage() == false;

        public void Launch()
        {
            SwitchStage();

            _isRunning = true;

            Launched?.Invoke();
        }

        public void SkipStage()
        {
            if (_stageCanBeSkiped.Value == false)
                throw new InvalidOperationException("Current stage can not be skiped");

            SwitchStage();
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Update(float deltaTime)
        {
            if (_isRunning == false)
                return;

            _stageProviderService.UpdateCurrent(deltaTime);
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
        }

        private void OnStageResultChanged(StageResults oldResult, StageResults newResult)
        {
            switch (newResult)
            {
                case StageResults.Completed:
                    OnStageComplete();
                    break;

                case StageResults.Uncompleted:
                    break;

                case StageResults.CanBeSkiped:
                    if (_stageProviderService.HasNextStage())
                        OnStageCanBeSkiped();
                    break;
            }
        }

        private void OnStageComplete()
        {
            if (_stageProviderService.HasNextStage())
                SwitchStage();
            else
                OnCycleComplete();
        }

        private void OnStageCanBeSkiped()
        {
            _stageCanBeSkiped.Value = true;

            _gameplayPopupService.OpenSkipStagePopup();

            Debug.Log($"Текущий стейдж может быть пропущен");
        }

        public void SwitchStage()
        {
            _stageCanBeSkiped.Value = false;

            _stageProviderService.SwitchToNext(out float remainingStageTime);

            if (remainingStageTime > 0)
                _earnGoldOnSkipStageService.Earn(remainingStageTime);

            _stageProviderService.StartCurrent();
            Debug.Log($"Запуск стейджа под номером: {_stageProviderService.CurrentStageNumber.Value}");
        }

        private void OnCycleComplete()
        {
            _isRunning = false;
            _stageCanBeSkiped.Value = false;
            _isCycleComplete.Value = true;
        }
    }
}
