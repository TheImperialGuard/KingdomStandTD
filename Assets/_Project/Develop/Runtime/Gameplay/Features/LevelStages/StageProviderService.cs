using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public class StageProviderService : IDisposable
    {
        private StagesFactory _stagesFactory;

        private IStagesSource _stagesSource;

        private IStage _currentStage;

        private ReactiveVariable<int> _currentStageNumber = new();
        private ReactiveVariable<StageResults> _currentStageResult = new();

        private IDisposable _stageEndedDisposable;
        private IDisposable _stageCanBeSkipedDisposable;

        public StageProviderService(StagesFactory stagesFactory, IStagesSource stagesSource)
        {
            _stagesFactory = stagesFactory;
            _stagesSource = stagesSource;

            // Номер растёт при запуске каждого стейджа, и первым запустится первый стейдж источника
            _currentStageNumber.Value = _stagesSource.FirstNumber - 1;
        }

        public IReadOnlyVariable<int> CurrentStageNumber => _currentStageNumber;

        public StageConfig NextStageConfig => _stagesSource.Get(_currentStageNumber.Value + 1);

        public IReadOnlyVariable<StageResults> CurrentStageResult => _currentStageResult;

        public IReadOnlyVariable<float> CurrentStageRemainingTime => _currentStage.RemainingTime;

        // У бесконечного режима стейджей без конца, и StagesCount у него нет
        public bool HasFiniteStages => _stagesSource.IsFinite;

        public int StagesCount => _stagesSource.Count;

        public bool HasNextStage() => _stagesSource.Has(_currentStageNumber.Value + 1);

        public void SwitchToNext(out float currentStageRemainingTime)
        {
            if (HasNextStage() == false)
                throw new InvalidOperationException();

            currentStageRemainingTime = 0f;

            if (_currentStage != null)
            {
                currentStageRemainingTime = _currentStage.RemainingTime.Value;

                CleanupCurrent();
            }

            _currentStageNumber.Value++;
            _currentStageResult.Value = StageResults.Uncompleted;

            _currentStage = _stagesFactory.Create(_stagesSource.Get(_currentStageNumber.Value));
        }

        public void StartCurrent()
        {
            _stageEndedDisposable = _currentStage.Completed.Subscribe(OnCurrentStageCompleted);
            _stageCanBeSkipedDisposable = _currentStage.CanBeSkiped.Subscribe(OnCurrentStageCanBeSkiped);

            _currentStage.Start();
        }

        public void UpdateCurrent(float deltaTime) => _currentStage.Update(deltaTime);

        public void CleanupCurrent() => _currentStage?.Cleanup();

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
