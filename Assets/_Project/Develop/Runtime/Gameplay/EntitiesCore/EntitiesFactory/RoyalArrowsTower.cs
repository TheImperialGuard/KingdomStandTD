using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateRoyalArrowsTower(Vector3 position, RoyalArrowsTowerConfig config, Dictionary<StatTypes, float> baseStats)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            Dictionary<StatTypes, float> modifiedStats = new(baseStats);

            entity
                .AddBaseStats(baseStats)
                .AddModifiedStats(modifiedStats)
                .AddCurrentTarget()
                .AddMaxTargets(new(1))
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.AttackDamage))
                .AddInstantAttackDamageType(new(config.DamageType))
                .AddInstantShootRange(new ReactiveVariable<float>(config.AttackRange))
                .AddInstantShotDirection()
                .AddAttackCooldownCurrentTime()
                .AddAttackCooldownInitialTime()
                .AddInAttackCooldown()
                .AddProjectileSpeed(new(config.ProjectileSpeed))
                .AddAttacksPerSecond(new ReactiveVariable<float>(modifiedStats[StatTypes.AttacksPerSecond]))
                .AddSubTowerCreator(CreateSubArrowsEntity);

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false));

            entity
                .AddCanStartAttack(canStartAttack);

            entity
                .AddSystem(new RangeZoneRadiusSyncSystem())
                .AddSystem(new AttackCooldownByAttackPerSecondStatSyncSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new ShootDirectionCalculateSystem())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new InstantShootSystem(this));

            return entity;
        }

        public Entity CreateSubArrowsEntity(Entity parent)
        {
            Entity entity = CreateEmpty();

            entity
                .AddShootPoint(parent.ShootPoint)
                .AddTransform(parent.Transform);

            entity
                .AddIsSubTower()
                .AddSubTowerParent(parent)
                .AddBaseStats(parent.BaseStats)
                .AddModifiedStats(parent.ModifiedStats)
                .AddCurrentTarget()
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInstantAttackDamage(parent.InstantAttackDamage)
                .AddInstantAttackDamageType(parent.InstantAttackDamageType)
                .AddInstantShootRange(parent.InstantShootRange)
                .AddInstantShotDirection()
                .AddAttackCooldownCurrentTime()
                .AddAttackCooldownInitialTime(parent.AttackCooldownInitialTime)
                .AddProjectileSpeed(parent.ProjectileSpeed)
                .AddInAttackCooldown()
                .AddAttacksPerSecond(parent.AttacksPerSecond);

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false));

            entity
                .AddCanStartAttack(canStartAttack);

            entity
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new ShootDirectionCalculateSystem())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new InstantShootSystem(this));

            return entity;
        }
    }
}
