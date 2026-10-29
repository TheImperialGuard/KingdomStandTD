using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateShootingTower(Vector3 position, ShootingTowerConfig config, Dictionary<StatTypes, float> baseStats)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            Dictionary<StatTypes, float> modifiedStats = new(baseStats);

            entity
                .AddBaseStats(baseStats)
                .AddModifiedStats(modifiedStats)
                .AddCurrentTarget()
                .AddAttackProcessInitialTime(new ReactiveVariable<float>(config.AttackProcessTime))
                .AddAttackProcessModifiedTime(new ReactiveVariable<float>(config.AttackProcessTime))
                .AddAttackProcessCurrentTime()
                .AddInAttackProcess()
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddAttackDelayTime(new ReactiveVariable<float>(config.AttackDelayTime))
                .AddAttackDelayModifiedTime(new ReactiveVariable<float>(config.AttackDelayTime))
                .AddAttackDelayEndEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.AttackDamage))
                .AddInstantAttackDamageType(new(config.DamageType))
                .AddInstantShootRange(new ReactiveVariable<float>(config.AttackRange))
                .AddAttackCooldownCurrentTime()
                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.AttackCooldown))
                .AddAttackCooldownModifiedTime(new ReactiveVariable<float>(config.AttackCooldown))
                .AddInAttackCooldown()
                .AddProjectileType(new(config.Projectile))
                .AddProjectileSpeed(new(config.ProjectileSpeed))
                .AddAttacksPerSecond(new ReactiveVariable<float>(modifiedStats[StatTypes.AttacksPerSecond]))
                .AddIsFrozen()
                .AddStatuses();

            entity
                .AddSystem(new RangeZoneRadiusSyncSystem())
                .AddSystem(new AttackTimeByAttackPerSecondStatSyncSystem())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new StatusesLifeCycleSystem(_container.Resolve<TimerServiceFactory>()))
                .AddSystem(new StatusesApplierSystem(_container.Resolve<TimerServiceFactory>()));

            return entity;
        }
    }
}
