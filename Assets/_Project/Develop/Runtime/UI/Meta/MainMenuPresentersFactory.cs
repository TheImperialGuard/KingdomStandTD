using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Meta.Features.Settings;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Settings;
using Assets._Project.Develop.Runtime.UI.Settings;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.UI.Meta
{
    public class MainMenuPresentersFactory
    {
        private readonly DIContainer _container;

        public MainMenuPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public LevelTilePresenter CreateLevelTilePresenter(LevelTileView view, int levelNumber)
        {
            return new LevelTilePresenter(
                view,
                _container.Resolve<SceneSwitcherService>(),
                _container.Resolve<ICoroutinesPerformer>(),
                levelNumber,
                _container.Resolve<LevelsProgressionService>());
        }

        public MainMenuScreenPresenter CreateMainMenuScreen(MainMenuScreenView view)
        {
            return new MainMenuScreenPresenter(
                view,
                _container.Resolve<MainMenuPopupService>(),
                _container.Resolve<HotkeysService>());
        }

        public LevelsMenuPopupPresenter CreateLevelsMenuPopupPresenter(LevelsMenuPopupView view)
        {
            return new LevelsMenuPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<RayShooterService>(),
                this,
                _container.Resolve<ViewsFactory>(),
                _container.Resolve<LevelsProgressionService>(),
                _container.Resolve<ConfigsProviderService>(),
                view);
        }

        public SettingsPopupPresenter CreateSettingsPopupPresenter(SettingsPopupView view)
        {
            return new SettingsPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<RayShooterService>(),
                view,
                this);
        }

        public AudioSettingsPresenter CreateAudioSettingsPresenter(AudioSettingsView view)
        {
            return new AudioSettingsPresenter(
                view,
                _container.Resolve<AudioHandler>(),
                _container.Resolve<AudioSettingsService>());
        }
    }
}
