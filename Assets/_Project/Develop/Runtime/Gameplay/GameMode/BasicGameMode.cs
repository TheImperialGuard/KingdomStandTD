using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    public class BasicGameMode : IGameMode
    {
        private ReactiveEvent<LevelResults> _end = new();

        private readonly StagesCycle _stagesCycle;
        private readonly WavesSpawner _wavesSpawner;

        private bool _isRunning;

        private CompositeCondition _winCondition = new();

        private List<IDisposable> _disposables = new();

        public BasicGameMode(StagesCycle stagesCycle, WavesSpawner wavesSpawner)
        {
            _stagesCycle = stagesCycle;
            _wavesSpawner = wavesSpawner;

            _winCondition
                .Add(new FuncCondition(() => _stagesCycle.IsCycleComplete.Value == true))
                .Add(new FuncCondition(() => wavesSpawner.IsSpawnComplete.Value == true))
                .Add(new FuncCondition(() => wavesSpawner.SpawnedEntitites.Count == 0));

            // также добавить loseCond и проверку в update -> processLose
        }

        public IReadOnlyEvent<LevelResults> End => _end;

        public void Start()
        {
            //открытие попапов - триггеров волн
            //подписка на нажатие на эти попапы OnStartStagesCycle

            _isRunning = true;

            OnStartStagesRequest(); //УБРАТЬ ПОСЛЕ РЕАЛИЗАЦИИ ПОПАПОВ СТАРТА
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

            //отписка от нажатия на попапы OnStartStagesCycle
            foreach (IDisposable disposable  in _disposables)
                disposable.Dispose();
        }

        private void OnStartStagesRequest()
        {
            //закрытие попапов

            _stagesCycle.Launch();
        }

        private void ProcessEndGame()
        {
            _isRunning = false;
        }

        private void ProcessWin()
        {
            ProcessEndGame();

            LevelResults levelResults = CalculateWinResults();

            _end.Invoke(levelResults);
        }

        private LevelResults CalculateWinResults()
        {
            LevelResults results;

            // проверить сервис хп игрока и выдать степень победы

            results = LevelResults.Perfect;

            return results;
        }
    }
}
