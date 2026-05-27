using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Infrastructure.EntryPoint;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Infrastructure.Boot
{
    public class GameEntryPoint : MonoBehaviour
    {
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
            //Show loading screen

            Debug.Log("Начинается инициализация сервисов");

            yield return container.Resolve<ConfigsProviderService>().LoadAsync();

            yield return new WaitForSeconds(1);

            Debug.Log("Завершается инициализация сервисов");

            //Hide loading screen
            //Switch scene
        }

        private void SetupAppSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}
