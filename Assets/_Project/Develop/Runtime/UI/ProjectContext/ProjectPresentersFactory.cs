using Assets._Project.Develop.Runtime.Configs.Encyclopedia;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext
{
    // Презентеры интерфейса, общего для всех сцен, — например, энциклопедии.
    public class ProjectPresentersFactory
    {
        private readonly DIContainer _container;

        public ProjectPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        // Без записи для открытия (startRecord = null) книга открывается на последнем разделе.
        public EncyclopediaPopupPresenter CreateEncyclopediaPopupPresenter(
            EncyclopediaPopupView view,
            EncyclopediaModes mode,
            ScriptableObject startRecord)
        {
            ConfigsProviderService configsProviderService = _container.Resolve<ConfigsProviderService>();
            EncyclopediaConfig encyclopediaConfig = configsProviderService.GetConfig<EncyclopediaConfig>();

            return new EncyclopediaPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<RayShooterService>(),
                view,
                mode,
                new EncyclopediaContents(encyclopediaConfig),
                encyclopediaConfig,
                configsProviderService.GetConfig<TowersAbilitiesListConfig>(),
                _container.Resolve<EncyclopediaSectionMemory>(),
                _container.Resolve<HotkeysService>(),
                _container.Resolve<ViewsFactory>(),
                startRecord);
        }
    }
}
