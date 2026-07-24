using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public class StagesCycle : IDisposable
    {
        private readonly StageProviderService _stageProviderService;

        private ReactiveVariable<bool> _isCycleComplete = new();

        private bool _isRunning;
        private bool _stageCanBeSkiped;

        private List<IDisposable> _disposables = new();

        public StagesCycle(StageProviderService stageProviderService)
        {
            _stageProviderService = stageProviderService;

            _disposables.Add(_stageProviderService.CurrentStageResult.Subscribe(OnStageResultChanged));
        }

        public IReadOnlyVariable<bool> IsCycleComplete => _isCycleComplete;

        public bool InLastStage => _stageProviderService.HasNextStage() == false;

        public void Launch()
        {
            SwitchStage();

            _isRunning = true;
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

            if (_stageCanBeSkiped == true)
            {
                if (Input.GetKeyDown(KeyCode.S))
                    SwitchStage();
            }
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
            _stageCanBeSkiped = true;

            // создать попапы скипа
            Debug.Log($"Текущий стейдж может быть пропущен");

        }

        private void SwitchStage()
        {
            _stageCanBeSkiped = false;
            _stageProviderService.SwitchToNext();
            _stageProviderService.StartCurrent();
            Debug.Log($"Запуск стейджа под номером: {_stageProviderService.CurrentStageNumber.Value}");
        }

        private void OnCycleComplete()
        {
            _isRunning = false;
            _stageCanBeSkiped = false;
            _isCycleComplete.Value = true;
        }
    }
}