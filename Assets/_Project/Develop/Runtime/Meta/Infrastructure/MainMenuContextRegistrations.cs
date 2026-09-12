using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuContextRegistrations
    {
        private static MainMenuInputArgs _inputArgs;

        public static void Process(DIContainer container, MainMenuInputArgs args)
        {
            _inputArgs = args;

            container.RegisterAsSingle(CreateMainMenuPresentersFactory);

            container.RegisterAsSingle(CreateMainMenuPopupService);

            container.RegisterAsSingle(CreateMainMenuUIRoot).NonLazy();

            container.RegisterAsSingle(CreateMainMenuScreenPresenter).NonLazy();
        }

        private static MainMenuPresentersFactory CreateMainMenuPresentersFactory(DIContainer c)
        {
            return new MainMenuPresentersFactory(c);
        }

        private static MainMenuPopupService CreateMainMenuPopupService(DIContainer c)
        {
            return new MainMenuPopupService(
                c.Resolve<ViewsFactory>(),
                c.Resolve<MainMenuUIRoot>(),
                c.Resolve<MainMenuPresentersFactory>());
        }

        private static MainMenuUIRoot CreateMainMenuUIRoot(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            MainMenuUIRoot mainMenuUIRootPrefab = resourcesAssetsLoader
                .Load<MainMenuUIRoot>("UI/Meta/MainMenuUIRoot");

            return GameObject.Instantiate(mainMenuUIRootPrefab);
        }

        private static MainMenuScreenPresenter CreateMainMenuScreenPresenter(DIContainer c)
        {
            MainMenuUIRoot uiRoot = c.Resolve<MainMenuUIRoot>();

            MainMenuScreenView view = c
                .Resolve<ViewsFactory>()
                .Create<MainMenuScreenView>(ViewIDs.MainMenuScreen, uiRoot.HUDLayer);

            MainMenuScreenPresenter presenter = c
                .Resolve<MainMenuPresentersFactory>()
                .CreateMainMenuScreen(view);

            return presenter;
        }
    }
}
