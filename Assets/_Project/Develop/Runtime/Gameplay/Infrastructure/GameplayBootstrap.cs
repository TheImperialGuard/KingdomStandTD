using System;
using System.Collections;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Unity.Profiling;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private GameplayInputArgs _inputArgs;

        private EntitiesLifeContext _entitiesLifeContext;
        private AIBrainsContext _brainsContext;
        private GameplayCycle _gameplayCycle;
        private RayShooterService _rayShooterService;
        private MusicSwitcherService _musicSwitcherService;
        private CameraMover _cameraMover;

        private static readonly ProfilerMarker _markerCameraMover = new("KS.CameraMover");
        private static readonly ProfilerMarker _markerGameplayCycle = new("KS.GameplayCycle");
        private static readonly ProfilerMarker _markerRayShooterService = new("KS.RayShooterService");
        private static readonly ProfilerMarker _markerBrainsContext = new("KS.BrainsContext");
        private static readonly ProfilerMarker _markerEntitiesLifeContext = new("KS.EntitiesLifeContext");

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)} type");

            _inputArgs = gameplayInputArgs;

            GameplayContextRegistrations.Process(_container, _inputArgs);
        }

        public override IEnumerator Initialize()
        {
            Debug.Log($"Выполнен переход на уровень {_inputArgs.LevelNumber}");

            Debug.Log("Инициализация геймплейной сцены");

            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _rayShooterService = _container.Resolve<RayShooterService>();
            _gameplayCycle = _container.Resolve<GameplayCycle>();
            _musicSwitcherService = _container.Resolve<MusicSwitcherService>();
            _cameraMover = _container.Resolve<CameraMover>();

            _gameplayCycle.Prepare();

            yield break;
        }

        public override void Run()
        {
            Debug.Log("Старт геймплейной сцены");

            _musicSwitcherService.SwitchFor(MusicContexts.GameplayPrepare);

            _gameplayCycle.Launch();
        }

        private void Update()
        {
            using (_markerCameraMover.Auto())
                _cameraMover?.Update();

            using (_markerGameplayCycle.Auto())
                _gameplayCycle?.Update(Time.deltaTime);

            using (_markerRayShooterService.Auto())
                _rayShooterService?.Update(Time.deltaTime);

            using (_markerEntitiesLifeContext.Auto())
                _entitiesLifeContext?.Update(Time.deltaTime);

            using (_markerBrainsContext.Auto())
                _brainsContext?.Update(Time.deltaTime);

            //if (Input.GetKeyDown(KeyCode.M))
            //{
            //    SceneSwitcherService sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            //    ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            //    coroutinesPerformer.StartPerform(sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs()));
            //}
        }
    }
}
