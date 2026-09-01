using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class AbilitiesFactory
    {
        private DIContainer _container;

        public AbilitiesFactory(DIContainer container)
        {
            _container = container;
        }

        public Ability CreateAbilityFor(Entity entity, AbilityConfig config, int currentLevel)
        {
            switch (config)
            {
                case AdditionalShootingTargetsAbilityConfig addTargetsAbilityConfig:
                    return new AdditionalShootingTargetsAbility(addTargetsAbilityConfig, entity, currentLevel);

                case PoisonedAttackAbilityConfig poisonedAttackAbilityConfig:
                    return new PoisonedAttackAbility(entity, poisonedAttackAbilityConfig, currentLevel);

                case OneShotAttackAbilityConfig oneShotAttackAbilityConfig:
                    return new OneShotAttackAbility(entity, oneShotAttackAbilityConfig, currentLevel, _container.Resolve<ProjectilesFactory>());

                case StunAbilityConfig stunAbilityConfig:
                    return new StunAbility(entity, stunAbilityConfig, currentLevel, _container.Resolve<AreaEntitiesDetectorService>(), _container.Resolve<StatusesFactory>());

                default:
                    throw new ArgumentException();
            }
        }
    }
}
