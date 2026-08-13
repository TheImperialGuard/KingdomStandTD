using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles
{
    public class ProjectilesFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly ConfigsProviderService _configsProviderService;

        public ProjectilesFactory(DIContainer container)
        {
            _container = container;
            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
        }

        public Entity Create(ProjectilesTypes type, Vector3 position, Vector3 direction, Entity owner)
        {
            Entity entity;

            ProjectileConfig config = _configsProviderService.GetConfig<ProjectilesListConfig>().GetProjectileConfigBy(type);

            switch (type)
            {
                case ProjectilesTypes.Arrow:
                    entity = _entitiesFactory.CreateArrowProjectile(position, direction, owner, config.PrefabPath);
                    break;

                case ProjectilesTypes.PoisonedArrow:
                    entity = _entitiesFactory.CreateArrowProjectile(position, direction, owner, config.PrefabPath);
                    break;

                default:
                    throw new ArgumentException($"Projectile type of {type} not supported in factory");
            }

            entity.AddIsProjectile();

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}