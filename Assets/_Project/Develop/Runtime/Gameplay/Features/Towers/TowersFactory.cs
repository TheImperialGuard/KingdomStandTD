using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    public class TowersFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly InteractiveActionsFactory _interactiveActionsFactory;

        private readonly EntitiesLifeContext _entitiesLifeContext;

        public TowersFactory(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _interactiveActionsFactory = _container.Resolve<InteractiveActionsFactory>();

            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        public Entity Create(Vector3 position, TowerConfig config)
        {
            Entity entity;

            switch (config)
            {
                case ArrowsTowerConfig arrowsTowerConfig:
                    entity = _entitiesFactory.CreateArrowsTower(position, arrowsTowerConfig);

                    _brainsFactory.CreateTowerBrain(entity, new NearestEnemyInRangeSelector(entity));

                    break;

                default:
                    throw new ArgumentException($"Not support {config.GetType()} type config");
            }
            
            IInteractAction selectAction = _interactiveActionsFactory.CreateSelectTowerAction(entity);

            entity
                .AddTeam(new ReactiveVariable<Teams>(Teams.Allies))
                .AddIsInteractable()
                .AddInteractRequest()
                .AddInteractEvent()
                .AddInteractiveAction(new(selectAction));

            ICompositeCondition canInteract = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            entity
                .AddCanInteract(canInteract);

            entity
                .AddSystem(new InteractSystem());

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
