using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
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

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            ICompositeCondition canRotate = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            entity
                .AddCanMove(canMove)
                .AddCanRotate(canRotate);

            entity
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem());

            entity
                .AddSystem(new BodyContactsDetectingSystem());

            return entity;
        }
    }
}
