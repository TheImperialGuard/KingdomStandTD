using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.GameMode;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayCycle : IDisposable
    {
        private readonly GameModesFactory _gameModesFactory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly LevelsProgressionService _levelsProgressionService;
        private readonly TowersPlaceholdersService _towersPlaceholdersService;
        private readonly PlayerInteractsService _playerInteractsService;
        private MusicSwitcherService _musicSwitcherService;
        private GameplayPopupService _gameplayPopupService;

        private readonly GameModes _gameModeType;
        private readonly int _levelNumber;

        private IGameMode _gameMode;

        private IDisposable _gameModeDisposable;

        public GameplayCycle(
            GameModesFactory gameModesFactory,
            ICoroutinesPerformer coroutinesPerformer,
            GameModes gameModeType,
            PlayerDataProvider playerDataProvider,
            LevelsProgressionService levelsProgressionService,
            int levelNumber,
            TowersPlaceholdersService towersPlaceholdersService,
            PlayerInteractsService playerInteractsService,
            MusicSwitcherService musicSwitcherService,
            GameplayPopupService gameplayPopupService)
        {
            _gameModesFactory = gameModesFactory;
            _coroutinesPerformer = coroutinesPerformer;
            _gameModeType = gameModeType;
            _playerDataProvider = playerDataProvider;
            _levelsProgressionService = levelsProgressionService;
            _levelNumber = levelNumber;
            _towersPlaceholdersService = towersPlaceholdersService;
            _playerInteractsService = playerInteractsService;
            _musicSwitcherService = musicSwitcherService;
            _gameplayPopupService = gameplayPopupService;
        }

        public void Prepare()
        {
            _towersPlaceholdersService.CreateAllPlaceholders();
        }

        public void Launch()
        {
            _gameMode = _gameModesFactory.Create(_gameModeType);

            _gameModeDisposable = _gameMode.End.Subscribe(OnGameModeEnded);

            _gameMode.Start();
            _playerInteractsService.Enable();
        }

        public void Update(float deltaTime) => _gameMode?.Update(deltaTime);

        public void Dispose() => _gameModeDisposable.Dispose();

        private void OnGameModeEnded(LevelResults results)
        {
            _playerInteractsService.Disable();

            if (results == LevelResults.Defeat)
                ProcessDefeat();
            else
                ProcessWin(results);
        }

        private void ProcessWin(LevelResults results)
        {
            _musicSwitcherService.SwitchFor(MusicContexts.GameplayWin, false);

            SaveLevelResults(results);

            _gameplayPopupService.OpenWinPopup(results);
        }

        private void ProcessDefeat()
        {
            _musicSwitcherService.SwitchFor(MusicContexts.GameplayDefeat, false);

            _gameplayPopupService.OpenDefeatPopup();
        }

        private void SaveLevelResults(LevelResults results)
        {
            _levelsProgressionService.AddLevelResultsToCompleted(_levelNumber, results);

            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());
        }
    }
}
