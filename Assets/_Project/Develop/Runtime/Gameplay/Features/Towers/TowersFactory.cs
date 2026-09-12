using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
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

        public Entity Create(Vector3 position, ShootingTowerConfig config, int level)
        {
            Entity entity;

            Dictionary<StatTypes, float> baseStats = new()
            {
                {StatTypes.AttacksPerSecond, config.AttackPerSecond },
            };

            entity = _entitiesFactory.CreateShootingTower(position, config, baseStats);

            ITargetSelector targetSelector = new ClosestToFinishEnemyInRangeSelector(entity);

            if (config.TowerType == TowerTypes.Cannon)
            {
                AddBallisticShooter(entity);

                targetSelector = new NearestEnemyInRangeSelector(entity);
            }
            else
            {
                AddBaseShooter(entity);
            }

            if (level >= 4)
            {
                AddAbilitiesFor(entity, config);
            }

            _brainsFactory.CreateTowerBrain(entity, targetSelector);

            IInteractAction selectAction = _interactiveActionsFactory.CreateSelectTowerAction(entity);

            entity
                .AddTowerType(new(config.TowerType))
                .AddTowerLevel(new(level))
                .AddTeam(new ReactiveVariable<Teams>(Teams.Allies))
                .AddIsInteractable()
                .AddInteractRequest()
                .AddInteractEvent()
                .AddInteractiveAction(new(selectAction))
                .AddSelfReleaseRequested(new(false));

            ICompositeCondition canInteract = new CompositeCondition()
                .Add(new FuncCondition(() => entity.SelfReleaseRequested.Value == false));

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
                new ClosestToFinishEnemyInRangeSelector(subEntity, subEntity.TargetsForExclude));

            _entitiesLifeContext.Add(subEntity);

            return subEntity;
        }

        private void AddBaseShooter(Entity entity)
        {
            entity
                .AddInstantShotDirection();

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false));

            entity
                .AddCanStartAttack(canStartAttack);

            entity
                .AddSystem(new ShootDirectionCalculateSystem())
                .AddSystem(new InstantShootSystem(_container.Resolve<ProjectilesFactory>()));
        }

        private void AddBallisticShooter(Entity entity)
        {
            entity
                .AddBallisticTrajectory(new())
                .AddBallisticTrajectoryMaxHeight(new(2.5f))
                .AddMustStunOnAttack(new(false))
                .AddMustCreateFragmentsOnProjectileDeath(new(false));

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false))
                .Add(new FuncCondition(() => entity.BallisticTrajectory.Value.isValid == true));

            entity
                .AddCanStartAttack(canStartAttack);

            entity
                .AddSystem(new BallisticTrajectoryCalculateSystem())
                .AddSystem(new BallisticShootSystem(
                    _container.Resolve<ProjectilesFactory>(),
                    _container.Resolve<AreaEntitiesDetectorService>(),
                    _container.Resolve<StatusesFactory>()));
        }

        private void AddAbilitiesFor(Entity entity, ShootingTowerConfig config)
        {
            entity
                .AddAbilities()
                .AddSystem(new AbilityOnAddActivatorSystem());

            switch (config.TowerType)
            {
                case TowerTypes.Arrows:
                    entity
                        .AddMaxTargets(new(1))
                        .AddSubTowerCreator(_entitiesFactory.CreateSubArrowsEntity)
                        .AddSystem(new CreateSubTowersByMaxTargetsSystem(this))
                        .AddLastingDamage(new())
                        .AddLastingDamageInitialTime(new())
                        .AddLastingDamageInterval(new());

                    break;

                case TowerTypes.Magic:

                    break;

                case TowerTypes.Cannon:

                    break;
            }
        }
    }
}
