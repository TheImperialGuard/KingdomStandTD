using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    // Общее у режимов игры: окно старта стейджей, поражение, когда у замка кончилось здоровье, и остановка боя в конце —
    // после итога стейджи и спавн больше ничего не запускают. Условие победы задаёт наследник.
    public abstract class GameModeBase : IGameMode
    {
        private ReactiveEvent<LevelResults> _end = new();

        private readonly GameplayPopupService _popupService;

        private readonly StagesCycle _stagesCycle;
        private readonly WavesSpawner _wavesSpawner;
        private readonly PlayerHealth _playerHealth;

        private bool _isRunning;

        private List<IDisposable> _disposables = new();

        protected GameModeBase(
            StagesCycle stagesCycle,
            WavesSpawner wavesSpawner,
            PlayerHealth playerHealth,
            GameplayPopupService popupService)
        {
            _stagesCycle = stagesCycle;
            _wavesSpawner = wavesSpawner;
            _playerHealth = playerHealth;
            _popupService = popupService;
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

            OnUpdate(deltaTime);
        }

        public void Dispose()
        {
            _isRunning = false;

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
        }

        // Кадр идущего боя, после стейджей: здесь наследник проверяет своё условие победы
        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected void ProcessEndGame(LevelResults levelResults)
        {
            _isRunning = false;

            _stagesCycle.Stop();
            _wavesSpawner.Stop();

            _end.Invoke(levelResults);

            Debug.Log($"Конец уровня с результатом: {levelResults}");
        }

        // Враги, уже стоящие у замка, могут снять здоровье и после итога — второй раз он не наступает
        private void OnPlayerHealthChanged(int oldValue, int newValue)
        {
            if (_isRunning && newValue <= 0)
                ProcessEndGame(LevelResults.Defeat);
        }
    }
}
