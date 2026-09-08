using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataRepository;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.KeysStorage;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.Serializers;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Infrastructure.EntryPoint
{
    public class ProjectContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            container.RegisterAsSingle(CreateResourcesAssetsLoader);

            container.RegisterAsSingle(CreateSceneLoaderService);

            container.RegisterAsSingle(CreateTimerServiceFactory);

            container.RegisterAsSingle(CreateConfigsProviderService);

            container.RegisterAsSingle(CreateSceneSwitcherService);

            container.RegisterAsSingle(CreateViewsFactory);

            container.RegisterAsSingle(CreatePlayerDataProvider);

            container.RegisterAsSingle<ISaveLoadService>(CreateSaveLoadService);

            container.RegisterAsSingle<ICoroutinesPerformer>(CreateCoroutinesPerformer);

            container.RegisterAsSingle(CreateMusicPlayer).NonLazy();

            container.RegisterAsSingle<ILoadingScreen>(CreateLoadingScreen);

            container.RegisterAsSingle(CreateLevelsProgressionService).NonLazy();
        }

        private static ResourcesAssetsLoader CreateResourcesAssetsLoader(DIContainer c) => new();

        private static SceneLoaderService CreateSceneLoaderService(DIContainer c) => new();

        private static TimerServiceFactory CreateTimerServiceFactory(DIContainer c) => new(c);

        private static ConfigsProviderService CreateConfigsProviderService(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            ResourcesConfigsLoader resourcesConfigsLoader = new(resourcesAssetsLoader);

            return new ConfigsProviderService(resourcesConfigsLoader);
        }

        private static SceneSwitcherService CreateSceneSwitcherService(DIContainer c)
        {
            return new SceneSwitcherService(
                c.Resolve<SceneLoaderService>(),
                c.Resolve<ILoadingScreen>(),
                c);
        }

        private static ViewsFactory CreateViewsFactory(DIContainer c)
        {
            return new ViewsFactory(c.Resolve<ResourcesAssetsLoader>());
        }

        private static SaveLoadService CreateSaveLoadService(DIContainer c)
        {
            IDataSerializer dataSerializer = new JsonSerializer();
            IDataKeysStorage dataKeysStorage = new MapDataKeysStorage();

            string saveFolderPath = Application.isEditor ? Application.dataPath : Application.persistentDataPath;

            IDataRepository dataRepository = new LocalFileDataRepository(saveFolderPath, "json");

            return new SaveLoadService(dataSerializer, dataKeysStorage, dataRepository);
        }

        private static PlayerDataProvider CreatePlayerDataProvider(DIContainer c)
        {
            ISaveLoadService saveLoadService = c.Resolve<ISaveLoadService>();

            return new PlayerDataProvider(saveLoadService);
        }

        private static LevelsProgressionService CreateLevelsProgressionService(DIContainer c)
        {
            return new LevelsProgressionService(c.Resolve<PlayerDataProvider>());
        }

        private static CoroutinesPerformer CreateCoroutinesPerformer(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            CoroutinesPerformer coroutinesPerformerPrefab = resourcesAssetsLoader
                .Load<CoroutinesPerformer>("Utilities/CoroutinesPerformer");

            return GameObject.Instantiate(coroutinesPerformerPrefab);
        }

        private static StandardLoadingScreen CreateLoadingScreen(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            StandardLoadingScreen standardLoadingScreen = resourcesAssetsLoader
                .Load<StandardLoadingScreen>("Utilities/StandardLoadingScreen");

            return GameObject.Instantiate(standardLoadingScreen);
        }

        private static MusicPlayer CreateMusicPlayer(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            MusicPlayer musicPlayerPrefab = resourcesAssetsLoader
                .Load<MusicPlayer>("Utilities/Audio/MusicPlayer");

            return GameObject.Instantiate(musicPlayerPrefab);
        }
    }
}
