using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Gameplay.GameMode;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
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

        public static void Process(DIContainer container, GameplayInputArgs args)
        {
            _inputArgs = args;

            container.RegisterAsSingle(CreateEntitiesFactory);

            container.RegisterAsSingle(CreateEntitiesLifeContext);

            container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();

            container.RegisterAsSingle(CreateCollidersRegistryService);

            container.RegisterAsSingle(CreateLevel).NonLazy();

            container.RegisterAsSingle(CreateGameplayWalletService).NonLazy();

            container.RegisterAsSingle(CreateBrainsFactory);

            container.RegisterAsSingle(CreateAIBrainsContext);

            container.RegisterAsSingle(CreateEnemiesFactory);

            container.RegisterAsSingle(CreateTowerFactory);

            container.RegisterAsSingle(CreateWavesSpawner);

            container.RegisterAsSingle(CreateStagesFactory);

            container.RegisterAsSingle(CreateStageProviderService);

            container.RegisterAsSingle(CreateGameModesFactory);

            container.RegisterAsSingle(CreateGameplayCycle);
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
                c.Resolve<ICoroutinesPerformer>());
        }

        private static StageProviderService CreateStageProviderService(DIContainer c)
        {
            return new StageProviderService(
                c.Resolve<StagesFactory>(),
                c.Resolve<Level>().EnemiesWavesStageConfigs);
        }

        private static StagesCycle CreateGameplayCycle(DIContainer c)
        {
            return new StagesCycle(c.Resolve<StageProviderService>());
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

        private static WalletService CreateGameplayWalletService(DIContainer c)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies = new()
            {
                {CurrencyTypes.Gold, new ReactiveVariable<int>(0) }
            };

            return new WalletService(currencies);
        }

        private static Level CreateLevel(DIContainer c)
        {
            int levelNumber = _inputArgs.LevelNumber;

            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();
            ConfigsProviderService configsProviderService = c.Resolve<ConfigsProviderService>();

            LevelsListConfig levelsListConfig = configsProviderService.GetConfig<LevelsListConfig>();
            LevelConfig levelConfig = levelsListConfig.GetBy(levelNumber);

            Level levelPrefab = resourcesAssetsLoader.Load<Level>(levelConfig.PrefabPath);

            Level levelInstance = GameObject.Instantiate(levelPrefab);

            return levelInstance;
        }
    }
}