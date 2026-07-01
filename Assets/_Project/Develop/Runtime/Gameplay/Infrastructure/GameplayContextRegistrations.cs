using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
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

            container.RegisterAsSingle(CreateLevelEnvironment).NonLazy();

            container.RegisterAsSingle(CreateGameplayWalletService).NonLazy();

            container.RegisterAsSingle(CreateBrainsFactory);

            container.RegisterAsSingle(CreateAIBrainsContext);

            container.RegisterAsSingle(CreateEnemiesFactory);

            container.RegisterAsSingle(CreateTowerFactory);
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

        private static EntitiesFactory CreateEntitiesFactory(DIContainer c)
            => new(c);

        private static EnemiesFactory CreateEnemiesFactory(DIContainer c)
            => new(c);

        private static BrainsFactory CreateBrainsFactory(DIContainer c)
            => new(c);

        private static TowersFactory CreateTowerFactory(DIContainer c)
            => new(c);

        private static WalletService CreateGameplayWalletService(DIContainer c)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies = new()
            {
                {CurrencyTypes.Gold, new ReactiveVariable<int>(0) }
            };

            return new WalletService(currencies);
        }

        private static Level CreateLevelEnvironment(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            Level level = resourcesAssetsLoader
                .Load<Level>("Prefabs/Levels/LevelOrigin");

            return GameObject.Instantiate(level);
        }
    }
}