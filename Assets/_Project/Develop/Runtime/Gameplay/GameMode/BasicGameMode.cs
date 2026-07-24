using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    public class BasicGameMode : IGameMode
    {
        private ReactiveEvent<LevelResults> _end = new();

        private readonly GameplayPopupService _popupService;

        private readonly StagesCycle _stagesCycle;
        private readonly WavesSpawner _wavesSpawner;
        private readonly PlayerHealth _playerHealth;

        private bool _isRunning;

        private CompositeCondition _winCondition = new();

        private List<IDisposable> _disposables = new();

        public BasicGameMode(
            StagesCycle stagesCycle,
            WavesSpawner wavesSpawner,
            PlayerHealth playerHealth,
            GameplayPopupService popupService)
        {
            _stagesCycle = stagesCycle;
            _wavesSpawner = wavesSpawner;
            _playerHealth = playerHealth;
            _popupService = popupService;

            CreateWinCondition();
        }

        public IReadOnlyEvent<LevelResults> End => _end;

        public void Start()
        {
            _popupService.OpenStartStagesPopup();

            _disposables.Add(_playerHealth.Current.Subscribe(OnPlayerHealthChanged));

            _isRunning = true;
        }

        public void Update(float deltaTime)
        {
            if (_isRunning == false)
                return;

            _stagesCycle.Update(deltaTime);

            if (_winCondition.Evaluate() == true)
                ProcessWin();
        }

        public void Dispose()
        {
            _isRunning = false;

            foreach (IDisposable disposable  in _disposables)
                disposable.Dispose();
        }

        private void CreateWinCondition()
        {
            _winCondition
                .Add(new FuncCondition(() => _stagesCycle.InLastStage == true))
                .Add(new FuncCondition(() => _wavesSpawner.IsSpawnComplete == true))
                .Add(new FuncCondition(() => _wavesSpawner.SpawnedEntitites.Count == 0));
        }

        private void OnPlayerHealthChanged(int oldValue, int newValue)
        {
            if (newValue <= 0)
                ProcessLose();
        }

        private void ProcessWin()
        {
            LevelResults levelResults = CalculateWinResults();

            ProcessEndGame(levelResults);
        }

        private void ProcessLose()
        {
            LevelResults levelResults = LevelResults.Defeat;

            ProcessEndGame(levelResults);
        }

        private void ProcessEndGame(LevelResults levelResults)
        {
            _isRunning = false;

            _end.Invoke(levelResults);

            Debug.Log($"Конец уровня с результатом: {levelResults}");
        }

        private LevelResults CalculateWinResults()
        {
            LevelResults results;

            int currentHealth = _playerHealth.Current.Value;
            int maxHealth = _playerHealth.Max.Value;

            float relation = (float)currentHealth / (float)maxHealth;

            if (relation == 1f)
                results = LevelResults.Perfect;
            else if (relation >= 0.5f)
                results = LevelResults.Average;
            else
                results = LevelResults.Bad;

            return results;
        }
    }
}
