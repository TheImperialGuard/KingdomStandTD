using Assets._Project.Develop.Runtime.Configs.Gameplay.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateTower(Vector3 position, TowerConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            entity
                .AddCurrentTarget()
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.AttackDamage))
                .AddInstantShootRange(new ReactiveVariable<float>(config.AttackRange))
                .AddInstantShotDirection()
                .AddAttackCancelEvent()
                .AddAttackCooldownCurrentTime()
                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.AttackCooldown))
                .AddInAttackCooldown();

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false));

            entity
                .AddCanStartAttack(canStartAttack);

            entity
                .AddSystem(new ShootDirectionCalculateSystem())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new InstantShootSystem(this));

            return entity;
        }
    }
}
