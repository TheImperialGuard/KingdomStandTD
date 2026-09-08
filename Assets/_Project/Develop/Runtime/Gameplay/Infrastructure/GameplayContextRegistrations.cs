using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Gameplay.GameMode;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        private static GameplayInputArgs _inputArgs;
        private static LevelConfig _levelConfig;

        public static void Process(DIContainer container, GameplayInputArgs args)
        {
            _inputArgs = args;
            _levelConfig = GetLevelConfig(container);

            container.RegisterAsSingle(CreateEntitiesFactory);

            container.RegisterAsSingle(CreateEntitiesLifeContext);

            container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();

            container.RegisterAsSingle(CreateCollidersRegistryService);

            container.RegisterAsSingle(CreateLevel).NonLazy();

            container.RegisterAsSingle(CreateGameplayWalletService);

            container.RegisterAsSingle(CreateTowersPurchaseService);

            container.RegisterAsSingle(CreateEarnGoldOnSkipStageService);

            container.RegisterAsSingle(CreatePlayerHealth);

            container.RegisterAsSingle(CreateBrainsFactory);

            container.RegisterAsSingle(CreateAIBrainsContext);

            container.RegisterAsSingle(CreateEnemiesFactory);

            container.RegisterAsSingle(CreateTowerFactory);

            container.RegisterAsSingle(CreateWavesSpawner);

            container.RegisterAsSingle(CreateStagesFactory);

            container.RegisterAsSingle(CreateStageProviderService);

            container.RegisterAsSingle(CreateGameModesFactory);

            container.RegisterAsSingle(CreateInteractiveActionsFactory);

            container.RegisterAsSingle(CreateGameplayPresentersFactory);

            container.RegisterAsSingle(CreateProjectilesFactory);

            container.RegisterAsSingle(CreateStatusesFactory);

            container.RegisterAsSingle(CreateAbilitiesFactory);

            container.RegisterAsSingle(CreateGameplayCycle);

            container.RegisterAsSingle(CreateStagesCycle);

            container.RegisterAsSingle(CreateTowersPlaceholdersService);

            container.RegisterAsSingle(CreateRayShooterService);

            container.RegisterAsSingle(CreateAreaEntitiesDetectorService);

            container.RegisterAsSingle(CreatePlayerInteractsService);

            container.RegisterAsSingle(CreateGameplayPopupService);

            container.RegisterAsSingle<IInputService>(CreateDesktopInput);

            container.RegisterAsSingle(CreateDealDamageToPlayerService).NonLazy();

            container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();

            container.RegisterAsSingle(CreateGameplayScreenPresenter).NonLazy();
        }

        private static CollidersRegistryService CreateCollidersRegistryService(DIContainer c) => new();

        private static EntitiesLifeContext CreateEntitiesLifeContext(DIContainer c) => new();

        private static AIBrainsContext CreateAIBrainsContext(DIContainer c) => new();

        private static MonoEntitiesFactory CreateMonoEntitiesFactory(DIContainer c)
        {
            return new MonoEntitiesFactory(
                c.Resolve<ResourcesAssetsLoader>(),
                c.Resolve<EntitiesLifeContext>(),
                c.Resolve<CollidersRegistryService>());
        }

        private static WavesSpawner CreateWavesSpawner(DIContainer c)
        {
            return new WavesSpawner(
                c.Resolve<EnemiesFactory>(),
                c.Resolve<TimerServiceFactory>(),
                c.Resolve<ICoroutinesPerformer>(),
                c.Resolve<EntitiesLifeContext>());
        }

        private static StageProviderService CreateStageProviderService(DIContainer c)
        {
            return new StageProviderService(
                c.Resolve<StagesFactory>(),
                c.Resolve<Level>().EnemiesWavesStageConfigs);
        }

        private static DealDamageToPlayerService CreateDealDamageToPlayerService(DIContainer c)
        {
            return new DealDamageToPlayerService(
                c.Resolve<PlayerHealth>(),
                c.Resolve<EntitiesLifeContext>());
        }

        private static GameplayCycle CreateGameplayCycle(DIContainer c)
        {
            return new GameplayCycle(
                c.Resolve<GameModesFactory>(),
                c.Resolve<ICoroutinesPerformer>(),
                _levelConfig.GameMode,
                c.Resolve<PlayerDataProvider>(),
                c.Resolve<LevelsProgressionService>(),
                _inputArgs.LevelNumber,
                c.Resolve<TowersPlaceholdersService>(),
                c.Resolve<PlayerInteractsService>(),
                c.Resolve<MusicSwitcherService>());
        }

        private static RayShooterService CreateRayShooterService(DIContainer c)
        {
            return new RayShooterService(c.Resolve<IInputService>());
        }

        private static AreaEntitiesDetectorService CreateAreaEntitiesDetectorService(DIContainer c)
        {
            return new AreaEntitiesDetectorService(c.Resolve<CollidersRegistryService>());
        }

        private static DesktopInput CreateDesktopInput(DIContainer c)
        {
            return new DesktopInput();
        }

        private static PlayerInteractsService CreatePlayerInteractsService(DIContainer c)
        {
            return new PlayerInteractsService(c.Resolve<RayShooterService>());
        }

        private static TowersPlaceholdersService CreateTowersPlaceholdersService(DIContainer c)
        {
            ConfigsProviderService configsProviderService = c.Resolve<ConfigsProviderService>();

            TowerPlaceholderConfig config = configsProviderService.GetConfig<TowerPlaceholderConfig>();

            return new TowersPlaceholdersService(
                c.Resolve<Level>(),
                c.Resolve<EntitiesFactory>(),
                config);
        }

        private static EntitiesFactory CreateEntitiesFactory(DIContainer c)
            => new(c);

        private static EnemiesFactory CreateEnemiesFactory(DIContainer c)
            => new(c);

        private static BrainsFactory CreateBrainsFactory(DIContainer c)
            => new(c);

        private static TowersFactory CreateTowerFactory(DIContainer c)
            => new(c);
        
        private static StagesFactory CreateStagesFactory(DIContainer c)
            => new(c);
        
        private static GameModesFactory CreateGameModesFactory(DIContainer c)
            => new(c);

        private static InteractiveActionsFactory CreateInteractiveActionsFactory(DIContainer c)
            => new(c);

        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer c)
            => new(c);

        private static ProjectilesFactory CreateProjectilesFactory(DIContainer c)
            => new(c);

        private static StatusesFactory CreateStatusesFactory(DIContainer c)
            => new(c);

        private static AbilitiesFactory CreateAbilitiesFactory(DIContainer c) 
            => new(c);

        private static WalletService CreateGameplayWalletService(DIContainer c)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies = new()
            {
                {CurrencyTypes.Gold, new ReactiveVariable<int>(_levelConfig.StartGold) }
            };

            return new WalletService(currencies);
        }

        private static TowersPurchaseService CreateTowersPurchaseService(DIContainer c)
        {
            return new TowersPurchaseService(
                c.Resolve<ConfigsProviderService>(),
                c.Resolve<WalletService>());
        }

        private static EarnGoldOnSkipStageService CreateEarnGoldOnSkipStageService(DIContainer c)
        {
            return new EarnGoldOnSkipStageService(
                c.Resolve<ConfigsProviderService>(),
                c.Resolve<WalletService>());
        }

        private static StagesCycle CreateStagesCycle(DIContainer c)
        {
            return new StagesCycle(
                c.Resolve<StageProviderService>(),
                c.Resolve<GameplayPopupService>(),
                c.Resolve<EarnGoldOnSkipStageService>());
        }

        private static GameplayPopupService CreateGameplayPopupService(DIContainer c)
        {
            return new(
                c.Resolve<ViewsFactory>(),
                c.Resolve<GameplayUIRoot>(),
                c.Resolve<GameplayPresentersFactory>());
        }

        private static PlayerHealth CreatePlayerHealth(DIContainer c)
        {
            int maxValue = _levelConfig.StartPlayerHealth;

            return new PlayerHealth(maxValue, maxValue);
        }

        private static Level CreateLevel(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            Level levelPrefab = resourcesAssetsLoader.Load<Level>(_levelConfig.PrefabPath);

            Level levelInstance = GameObject.Instantiate(levelPrefab);

            return levelInstance;
        }

        private static LevelConfig GetLevelConfig(DIContainer c)
        {
            int levelNumber = _inputArgs.LevelNumber;

            ConfigsProviderService configsProviderService = c.Resolve<ConfigsProviderService>();

            LevelsListConfig levelsListConfig = configsProviderService.GetConfig<LevelsListConfig>();
            LevelConfig levelConfig = levelsListConfig.GetBy(levelNumber);

            return levelConfig;
        }

        private static GameplayUIRoot CreateGameplayUIRoot(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            GameplayUIRoot gameplayUIRootPrefab = resourcesAssetsLoader
                .Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot");

            return GameObject.Instantiate(gameplayUIRootPrefab);
        }

        private static GameplayScreenPresenter CreateGameplayScreenPresenter(DIContainer c)
        {
            GameplayUIRoot uiRoot = c.Resolve<GameplayUIRoot>();

            GameplayScreenView view = c
                .Resolve<ViewsFactory>()
                .Create<GameplayScreenView>(ViewIDs.GameplayScreen, uiRoot.HUDLayer);

            GameplayScreenPresenter presenter = c
                .Resolve<GameplayPresentersFactory>()
                .CreateGameplayScreenPresenter(view);

            return presenter;
        }
    }
}