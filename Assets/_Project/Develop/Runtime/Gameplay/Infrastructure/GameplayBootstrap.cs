using System;
using System.Collections;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.RoutesPreview;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Unity.Profiling;
using UnityEngine;
#if KS_PERF_PROBE
using Assets._Project.Develop.Runtime.Gameplay.Features.PerformanceProbe;
#endif

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
        private RoutesPreviewService _routesPreviewService;
        private HotkeysService _hotkeysService;
        private ParticleEffectsPool _particleEffectsPool;

#if KS_PERF_PROBE
        private PerformanceProbeService _performanceProbeService;
#endif

        private static readonly ProfilerMarker _markerHotkeysService = new("KS.HotkeysService");
        private static readonly ProfilerMarker _markerCameraMover = new("KS.CameraMover");
        private static readonly ProfilerMarker _markerGameplayCycle = new("KS.GameplayCycle");
        private static readonly ProfilerMarker _markerRoutesPreviewService = new("KS.RoutesPreviewService");
        private static readonly ProfilerMarker _markerRayShooterService = new("KS.RayShooterService");
        private static readonly ProfilerMarker _markerBrainsContext = new("KS.BrainsContext");
        private static readonly ProfilerMarker _markerEntitiesLifeContext = new("KS.EntitiesLifeContext");
        private static readonly ProfilerMarker _markerParticleEffectsPool = new("KS.ParticleEffectsPool");

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
            _routesPreviewService = _container.Resolve<RoutesPreviewService>();
            _hotkeysService = _container.Resolve<HotkeysService>();
            _particleEffectsPool = _container.Resolve<ParticleEffectsPool>();

            #if KS_PERF_PROBE
                _performanceProbeService = _container.Resolve<PerformanceProbeService>();
            #endif

            // Пока виден экран загрузки: первые враги каждого вида возьмут объект из пула, а не создадут его посреди боя
            _container.Resolve<EnemiesPrewarmer>().Prewarm();

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
            using (_markerHotkeysService.Auto())
                _hotkeysService?.Update();

            using (_markerCameraMover.Auto())
                _cameraMover?.Update();

            using (_markerGameplayCycle.Auto())
                _gameplayCycle?.Update(Time.deltaTime);

            using (_markerRoutesPreviewService.Auto())
                _routesPreviewService?.Update(Time.deltaTime);

            using (_markerRayShooterService.Auto())
                _rayShooterService?.Update(Time.deltaTime);

            using (_markerEntitiesLifeContext.Auto())
                _entitiesLifeContext?.Update(Time.deltaTime);

            using (_markerBrainsContext.Auto())
                _brainsContext?.Update(Time.deltaTime);

            using (_markerParticleEffectsPool.Auto())
                _particleEffectsPool?.Update();

            #if KS_PERF_PROBE
                // Время кадра без масштаба: замер идёт и на паузе, и на ускорении
                _performanceProbeService?.Update(Time.unscaledDeltaTime);
            #endif

            //if (Input.GetKeyDown(KeyCode.M))
            //{
            //    SceneSwitcherService sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            //    ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            //    coroutinesPerformer.StartPerform(sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs()));
            //}
        }
    }
}
