using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        public StageProviderService(StagesFactory stagesFactory, IReadOnlyList<StageConfig> stagesConfigs)
        {
            _stagesFactory = stagesFactory;
            _stagesConfigs = new(stagesConfigs);
        }

        public IReadOnlyVariable<int> CurrentStageNumber => _currentStageNumber;

        public IReadOnlyVariable<StageResults> CurrentStageResult => _currentStageResult;

        public int StagesCount => _stagesConfigs.Count;

        public bool HasNextStage() => _currentStageNumber.Value < StagesCount;

        public void SwitchToNext()
        {
            if (HasNextStage() == false)
                throw new InvalidOperationException();

            if (_currentStage != null)
                CleanupCurrent();

            _currentStageNumber.Value++;
            _currentStageResult.Value = StageResults.Uncompleted;

            _currentStage = _stagesFactory.Create(_stagesConfigs[_currentStageNumber.Value - 1]);
        }

        public void StartCurrent()
        {
            _stageEndedDisposable = _currentStage.Completed.Subscribe(OnCurrentStageCompleted);

            _currentStage.Start();
        }

        public void UpdateCurrent(float deltaTime) => _currentStage.Update(deltaTime);

        public void CleanupCurrent() => _currentStage.Cleanup();

        public void Dispose()
        {
            _currentStage?.Dispose();

            _stageEndedDisposable?.Dispose();
        }

        private void OnCurrentStageCompleted()
        {
            _currentStageResult.Value = StageResults.Completed;
        }
    }
}
