using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public class StageProviderService : IDisposable
    {
        private StagesFactory _stagesFactory;

        private List<StageConfig> _stagesConfigs;

        private IStage _currentStage;

        private ReactiveVariable<int> _currentStageNumber = new();
        private ReactiveVariable<StageResults> _currentStageResult = new();

        private IDisposable _stageEndedDisposable;
        private IDisposable _stageCanBeSkipedDisposable;

        public StageProviderService(StagesFactory stagesFactory, IReadOnlyList<StageConfig> stagesConfigs)
        {
            _stagesFactory = stagesFactory;
            _stagesConfigs = new(stagesConfigs);
        }

        public IReadOnlyVariable<int> CurrentStageNumber => _currentStageNumber;

        public StageConfig NextStageConfig => _stagesConfigs[_currentStageNumber.Value];

        public IReadOnlyVariable<StageResults> CurrentStageResult => _currentStageResult;

        public int StagesCount => _stagesConfigs.Count;

        public bool HasNextStage() => _currentStageNumber.Value < StagesCount;

        public void SwitchToNext(out float currentStageRemainingTime)
        {
            if (HasNextStage() == false)
                throw new InvalidOperationException();

            currentStageRemainingTime = 0f;

            if (_currentStage != null)
            {
                currentStageRemainingTime = _currentStage.RemainingTime;

                CleanupCurrent();
            }

            _currentStageNumber.Value++;
            _currentStageResult.Value = StageResults.Uncompleted;

            _currentStage = _stagesFactory.Create(_stagesConfigs[_currentStageNumber.Value - 1]);
        }

        public void StartCurrent()
        {
            _stageEndedDisposable = _currentStage.Completed.Subscribe(OnCurrentStageCompleted);
            _stageCanBeSkipedDisposable = _currentStage.CanBeSkiped.Subscribe(OnCurrentStageCanBeSkiped);

            _currentStage.Start();
        }

        public void UpdateCurrent(float deltaTime) => _currentStage.Update(deltaTime);

        public void CleanupCurrent() => _currentStage.Cleanup();

        public void Dispose()
        {
            _currentStage?.Dispose();

            _stageEndedDisposable?.Dispose();
            _stageCanBeSkipedDisposable?.Dispose();
        }

        private void OnCurrentStageCompleted()
        {
            _currentStageResult.Value = StageResults.Completed;
        }

        private void OnCurrentStageCanBeSkiped()
        {
            _currentStageResult.Value = StageResults.CanBeSkiped;
        }
    }
}
