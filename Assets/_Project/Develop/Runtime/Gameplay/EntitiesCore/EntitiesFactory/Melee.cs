using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateMelee(Vector3 position, MeleeConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            entity
                .AddMoveDirection()
                .AddMoveSpeed(new(config.MoveSpeed))
                .AddIsMoving()
                .AddRotationDirection()
                .AddRotationSpeed(new(config.RotateSpeed));

            entity
                .AddContactsDetectingMask(Layers.TriggersMask)
                .AddContactCollidersBuffer(new Buffer<Collider>(64));

            entity
                .AddMaxHealth(new(config.MaxHealth))
                .AddCurrentHealth(new(config.MaxHealth))
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddIsDead()
                .AddDeathProcessInitialTime(new(config.DeathProcessTime))
                .AddDeathProcessCurrentTime()
                .AddInDeathProcces();

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canRotate = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            entity
                .AddCanMove(canMove)
                .AddCanRotate(canRotate)
                .AddCanApplyDamage(canApplyDamage)
                .AddMustDie(mustDie);

            entity
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem())
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem());

            entity
                .AddSystem(new BodyContactsDetectingSystem());

            return entity;
        }
    }
}
