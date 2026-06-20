using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class EnemiesFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;

        private readonly EntitiesLifeContext _entitiesLifeContext;

        public EnemiesFactory(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();

            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        public Entity Create(Vector3 position, EntityConfig config, IReadOnlyList<Waypoint> path)
        {
            Entity entity;

            switch (config)
            {
                case MeleeConfig meleeConfig:
                    entity = _entitiesFactory.CreateMelee(position, meleeConfig);

                    entity
                        .AddWaypoints(new(path))
                        .AddCurrentWaypoint()
                        .AddReachedWaypoints(new())
                        .AddIsPathFinished();

                    entity.
                        AddSystem(new WaypointsNavigationSystem());

                    _brainsFactory.CreateMeleeBrain(entity);

                    break;

                default:
                    throw new ArgumentException($"Not support {config.GetType()} type config");
            }

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
