using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System;
using System.Collections;
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

        private Level _level;

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
            _level = _container.Resolve<Level>();
            _rayShooterService = _container.Resolve<RayShooterService>();
            _gameplayCycle = _container.Resolve<GameplayCycle>();

            _gameplayCycle.Prepare();

            yield break;
        }

        public override void Run()
        {
            Debug.Log("Старт геймплейной сцены");

            _gameplayCycle.Launch();
        }

        private void Update()
        {
            _gameplayCycle?.Update(Time.deltaTime);
            _entitiesLifeContext?.Update(Time.deltaTime);
            _brainsContext?.Update(Time.deltaTime);
            _rayShooterService?.Update(Time.deltaTime);
            
            if (_rayShooterService.LastHitInfo.Value.collider.gameObject.TryGetComponent(out MonoEntity monoEntity))
            {
                if (monoEntity.LinkedEntity.TryGetComponent(out BoxColliderComponent boxCollider))
                {
                    TowersFactory towersFactory = _container.Resolve<TowersFactory>();

                    TowerConfig towerConfig = _container.Resolve<ResourcesAssetsLoader>().Load<TowerConfig>("Configs/Gameplay/Entities/Towers/Arrows/ArrowsTowerConfig_level_1");

                    towersFactory.Create(_rayShooterService.LastHitInfo.Value.collider.transform.position, towerConfig);

                    _rayShooterService.LastHitInfo.Value.collider.gameObject.SetActive(false);

                    _rayShooterService.Cleanup();
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneSwitcherService sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
                ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

                coroutinesPerformer.StartPerform(sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs(true)));
            }
        }
    }
}
