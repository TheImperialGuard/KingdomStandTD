using System.Collections;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Infrastructure.EntryPoint;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;
#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
#endif

namespace Assets._Project.Develop.Runtime.Infrastructure.Boot
{
    public class GameEntryPoint : MonoBehaviour
    {
        private const int TargetFrameRate = 60;
        private const int WebVSyncCount = 1;

        private void Awake()
        {
            Debug.Log("Старт проекта, сетап настроек");

            SetupAppSettings();

            Debug.Log("Регистрация сервисов глобального контекста");

            DIContainer projectContainer = new();

            ProjectContextRegistrations.Process(projectContainer);

            projectContainer.Initialize();

            projectContainer.Resolve<ICoroutinesPerformer>().StartPerform(Initialize(projectContainer));
        }

        private IEnumerator Initialize(DIContainer container)
        {
            ILoadingScreen loadingScreen = container.Resolve<ILoadingScreen>();
            SceneSwitcherService sceneSwitcherService = container.Resolve<SceneSwitcherService>();
            PlayerDataProvider playerDataProvider = container.Resolve<PlayerDataProvider>();

            loadingScreen.Show();

            Debug.Log("Начинается инициализация сервисов");

            yield return container.Resolve<ConfigsProviderService>().LoadAsync();

            bool isPlayerDataSaveExists = false;

            yield return playerDataProvider.ExistsAsync(result => isPlayerDataSaveExists = result);

            if (isPlayerDataSaveExists)
                yield return playerDataProvider.LoadAsync();
            else
                playerDataProvider.Reset();

            yield return null;

            Debug.Log("Завершается инициализация сервисов");

            loadingScreen.Hide();

            #if UNITY_EDITOR
                // Плейтест из редактора уровней: вместо главного меню сразу открывается его уровень
                if (PlaytestHooks.TryTakeStartArgs(out GameplayInputArgs playtestArgs))
                {
                    yield return sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, playtestArgs);
                    yield break;
                }
            #endif

            yield return sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs(Scenes.GameEntryPoint));
        }

        private void SetupAppSettings()
        {
            SetupFrameRate();
            SetupScreenOrientation();
        }

        // На вебе кадры задаёт цикл отрисовки браузера: с targetFrameRate они идут неровно
        private void SetupFrameRate()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                QualitySettings.vSyncCount = WebVSyncCount;
                return;
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFrameRate;
        }

        private void SetupScreenOrientation()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
}
