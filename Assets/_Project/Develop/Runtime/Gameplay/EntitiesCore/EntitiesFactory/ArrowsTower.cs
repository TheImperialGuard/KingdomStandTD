using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateArrowsTower(Vector3 position, ArrowsTowerConfig config, Dictionary<StatTypes, float> baseStats)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            Dictionary<StatTypes, float> modifiedStats = new(baseStats);

            entity
                .AddBaseStats(baseStats)
                .AddModifiedStats(modifiedStats)
                .AddCurrentTarget()
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.AttackDamage))
                .AddInstantAttackDamageType(new(config.DamageType))
                .AddInstantShootRange(new ReactiveVariable<float>(config.AttackRange))
                .AddInstantShotDirection()
                .AddAttackCooldownCurrentTime()
                .AddAttackCooldownInitialTime()
                .AddProjectileSpeed(new(config.ProjectileSpeed))
                .AddInAttackCooldown()
                .AddAttacksPerSecond(new ReactiveVariable<float>(modifiedStats[StatTypes.AttacksPerSecond]));

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
    }
}
