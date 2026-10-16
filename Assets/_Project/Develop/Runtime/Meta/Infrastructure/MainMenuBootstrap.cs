using System;
using System.Collections;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu;
using Assets._Project.Develop.Runtime.Utilities.Adverts;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private MainMenuInputArgs _inputArgs;

        private MusicSwitcherService _musicSwitcherService;
        private HotkeysService _hotkeysService;
        private MainMenuScreenPresenter _mainMenuScreenPresenter;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            if (sceneArgs is not MainMenuInputArgs mainMenuInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(MainMenuInputArgs)} type");

            _inputArgs = mainMenuInputArgs;

            MainMenuContextRegistrations.Process(_container, _inputArgs);
        }

        public override IEnumerator Initialize()
        {
            Debug.Log("Инициализация сцены главного меню");

            _musicSwitcherService = _container.Resolve<MusicSwitcherService>();
            _hotkeysService = _container.Resolve<HotkeysService>();
            _mainMenuScreenPresenter = _container.Resolve<MainMenuScreenPresenter>();

            yield break;
        }

        public override void Run()
        {
            Debug.Log("Старт сцены главного меню");

            if (_inputArgs.PreviousScene == Scenes.Gameplay)
            {
                _container.Resolve<IAdvertsService>().OpenFullScreenAdvert();
                _mainMenuScreenPresenter.OpenPanel(MainMenuPanels.Play);
            }

            _musicSwitcherService.SwitchFor(MusicContexts.MainMenu);
        }

        private void Update()
        {
            _hotkeysService?.Update();


            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    SceneSwitcherService sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            //    ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            //    coroutinesPerformer.StartPerform(sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(1)));
            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    PlayerDataProvider dataProvider = _container.Resolve<PlayerDataProvider>();
            //    ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            //    coroutinesPerformer.StartPerform(dataProvider.SaveAsync());
            //}
        }
    }
}
