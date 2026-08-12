using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

        public Entity Create(Vector3 position, TowerConfig config, int level)
        {
            Entity entity;

            Dictionary<StatTypes, float> baseStats = new()
            {
                {StatTypes.AttacksPerSecond, config.AttackPerSecond },
            };

            switch (config)
            {
                case ArrowsTowerConfig arrowsTowerConfig:
                    entity = _entitiesFactory.CreateArrowsTower(position, arrowsTowerConfig, baseStats);

                    entity
                        .AddTowerType(new(TowerTypes.Arrows))
                        .AddTowerLevel(new(level));

                    _brainsFactory.CreateTowerBrain(entity, new NearestEnemyInRangeSelector(entity));

                    break;

                case RoyalArrowsTowerConfig royalArrowsTowerConfig:
                    entity = _entitiesFactory.CreateRoyalArrowsTower(position, royalArrowsTowerConfig, baseStats);

                    entity
                        .AddTowerType(new(TowerTypes.Arrows))
                        .AddTowerLevel(new(level))
                        .AddAbilities(new());

                    entity
                        .AddSystem(new AbilityOnAddActivatorSystem())
                        .AddSystem(new CreateSubTowersByMaxTargetsSystem(this));

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
                .AddInteractiveAction(new(selectAction))
                .AddSelfReleaseRequested(new(false));

            ICompositeCondition canInteract = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.SelfReleaseRequested.Value == true));

            entity
                .AddCanInteract(canInteract)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                .AddSystem(new InteractSystem());

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public Entity CreateSubTowerFor(Entity parent, List<ReactiveVariable<Entity>> targetsForExclude = null)
        {
            Entity subEntity = parent.SubTowerCreator.Invoke(parent);

            subEntity
                .AddTeam(parent.Team)
                .AddSelfReleaseRequested(new(false))
                .AddTargetsForExclude(new(targetsForExclude));

            ICompositeCondition mustSelfRelease = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => subEntity.SelfReleaseRequested.Value == true))
                .Add(new FuncCondition(() => parent.SelfReleaseRequested.Value == true));

            subEntity
                .AddMustSelfRelease(mustSelfRelease);

            subEntity
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _brainsFactory.CreateTowerBrain(
                subEntity,
                new NearestEnemyInRangeSelector(subEntity, subEntity.TargetsForExclude));

            _entitiesLifeContext.Add(subEntity);

            return subEntity;
        }
    }
}
