using Assets._Project.Develop.Runtime.Gameplay.Features.ContactDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateArrowProjectile(Vector3 position, Vector3 direction, Entity owner, string prefabPath)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, prefabPath);

            entity
                .AddOwner(new ReactiveVariable<Entity>(owner))
                .AddMoveDirection(new ReactiveVariable<Vector3>(direction))
                .AddMoveSpeed(new ReactiveVariable<float>(owner.ProjectileSpeed.Value))
                .AddIsMoving()
                .AddRotationDirection(new ReactiveVariable<Vector3>(direction))
                .AddRotationSpeed(new ReactiveVariable<float>(9999))
                .AddIsDead()
                .AddContactsDetectingMask(Layers.CharacterMask | Layers.DeathZoneMask)
                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))
                .AddDeathMask(Layers.DeathZoneMask)
                .AddIsTouchDeathMask()
                .AddIsTouchAnotherTeam()
                .AddBodyContactDamage(new ReactiveVariable<float>(owner.InstantAttackDamage.Value))
                .AddInstantAttackDamageType(new(owner.InstantAttackDamageType.Value))
                .AddTeam(new ReactiveVariable<Teams>(owner.Team.Value));

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canRotate = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value), 0)
                .Add(new FuncCondition(() => entity.IsTouchAnotherTeam.Value), 10, LogicOperations.Or);

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value));

            entity
                .AddCanMove(canMove)
                .AddCanRotate(canRotate)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem())
                .AddSystem(new SweptBodyContactsDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new AnotherTeamTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            return entity;
        }

        public Entity CreateBallisticProjectile(Vector3 position, Vector3 direction, Entity owner, string prefabPath)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, prefabPath);

            entity
                .AddOwner(new ReactiveVariable<Entity>(owner))
                .AddIsDead()
                .AddContactsDetectingMask(Layers.EnviromentMask | Layers.DeathZoneMask)
                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(Layers.EnviromentMask | Layers.DeathZoneMask)
                .AddIsTouchDeathMask()
                .AddInstantAttackDamage(new ReactiveVariable<float>(owner.InstantAttackDamage.Value))
                .AddInstantAttackDamageType(new(owner.InstantAttackDamageType.Value))
                .AddTeam(new ReactiveVariable<Teams>(owner.Team.Value));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value), 0);

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new SweptBodyContactsDetectingSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            return entity;
        }
    }
}
