using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        // Нижняя граница времени полёта: при цели почти вплотную снежок не должен появиться у неё мгновенно.
        private const float MinSnowballFlightDuration = 0.3f;

        // Снежок летит дугой по времени, без физики, и по прилёте замораживает цель. Длительность заморозки копируется:
        // бросающий может освободиться раньше, чем снежок долетит.
        public Entity CreateSnowball(
            Vector3 position,
            Vector3 targetPoint,
            Entity target,
            float speed,
            float arcHeight,
            float freezeDuration,
            string prefabPath)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.CreatePooled(entity, position, prefabPath);

            Vector3 groundOffset = targetPoint - position;
            groundOffset.y = 0;

            float flightDuration = Mathf.Max(groundOffset.magnitude / speed, MinSnowballFlightDuration);

            entity
                .AddArcFlightStartPoint(new ReactiveVariable<Vector3>(position))
                .AddArcFlightEndPoint(new ReactiveVariable<Vector3>(targetPoint))
                .AddArcFlightHeight(new ReactiveVariable<float>(arcHeight))
                .AddArcFlightDuration(new ReactiveVariable<float>(flightDuration))
                .AddArcFlightCurrentTime()
                .AddIsArcFlightFinished()
                .AddFreezeTarget(new ReactiveVariable<Entity>(target))
                .AddFreezeDuration(new ReactiveVariable<float>(freezeDuration))
                .AddIsDead();

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsArcFlightFinished.Value));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new ArcFlightSystem())
                .AddSystem(new FreezeTargetOnArrivalSystem(_container.Resolve<StatusesFactory>()))
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            return entity;
        }
    }
}
