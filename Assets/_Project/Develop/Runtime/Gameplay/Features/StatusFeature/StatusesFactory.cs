using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class StatusesFactory
    {
        private readonly DIContainer _container;

        public StatusesFactory(DIContainer container)
        {
            _container = container;
        }

        public Status CreateFor(Entity entity, StatusesTypes type, Entity source, float initialTime, float interval)
        {
            Status status;

            IStatusEffect statusEffect = type switch
            {
                StatusesTypes.LastingDamage => new TakeDamageStatusEffect(source.LastingDamage),
                StatusesTypes.Stun => new StunStatusEffect(),
                _ => throw new ArgumentException($"{type} not supported in statuses factory"),
            };

            status = new Status(
                statusEffect,
                entity,
                initialTime,
                interval,
                type);

            return status;
        }
    }
}
