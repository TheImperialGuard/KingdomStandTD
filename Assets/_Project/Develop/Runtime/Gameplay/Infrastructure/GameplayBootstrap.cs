using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
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

            yield break;
        }

        public override void Run()
        {
            Debug.Log("Старт геймплейной сцены");

            Level level = _container.Resolve<Level>();

            EnemiesFactory enemiesFactory = _container.Resolve<EnemiesFactory>();
            TowersFactory towersFactory = _container.Resolve<TowersFactory>();

            MeleeConfig meleeConfig = _container.Resolve<ResourcesAssetsLoader>().Load<MeleeConfig>("Configs/Gameplay/Entities/Enemies/TestMelee");
            TowerConfig towerConfig = _container.Resolve<ResourcesAssetsLoader>().Load<TowerConfig>("Configs/Gameplay/Entities/Towers/TestTowerConfig");

            Vector3 spawnPos = new Vector3(
                level.RoadPaths[0].Waypoints[0].transform.position.x,
                level.RoadPaths[0].Waypoints[0].transform.position.y,
                level.RoadPaths[0].Waypoints[0].transform.position.z - 1);

            enemiesFactory.Create(spawnPos, meleeConfig, level.RoadPaths[0].Waypoints);
            towersFactory.Create(level.TowerTiles[0].TowerPosition, towerConfig);
        }

        private void Update()
        {
            _brainsContext?.Update(Time.deltaTime);
            _entitiesLifeContext?.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneSwitcherService sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
                ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

                coroutinesPerformer.StartPerform(sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, new MainMenuInputArgs(true)));
            }
        }
    }
}
