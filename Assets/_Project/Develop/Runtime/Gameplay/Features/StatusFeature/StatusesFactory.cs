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

        public Status CreateFor(Entity entity, StatusesTypes type, Entity source)
        {
            Status status;
            IStatusEffect statusEffect;

            switch (type)
            {
                case StatusesTypes.LastingDamage:
                    statusEffect = new TakeDamageStatusEffect(source.LastingDamage);

                    status = new Status(
                        statusEffect, 
                        entity, 
                        source.LastingDamageInitialTime.Value, 
                        source.LastingDamageInterval.Value,
                        type);

                    break;

                default:
                    throw new ArgumentException($"{type} not supported in statuses factory");
            }

            return status;
        }
    }
}
