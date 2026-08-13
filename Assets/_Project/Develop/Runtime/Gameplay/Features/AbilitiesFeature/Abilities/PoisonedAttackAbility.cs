using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class PoisonedAttackAbility : Ability
    {
        private readonly Entity _entity;
        private readonly PoisonedAttackAbilityConfig _config;

        public PoisonedAttackAbility(
            Entity entity,
            PoisonedAttackAbilityConfig config,
            int currentLevel) : base(config.ID, config.MaxLevel, currentLevel)
        {
            _entity = entity;
            _config = config;
        }

        public override void Activate()
        {
            _entity.AddLastingDamage(new(_config.GetDamageByLevel(CurrentLevel.Value)));
            _entity.AddLastingDamageInitialTime(new(_config.GetEffectTimeByLevel(CurrentLevel.Value)));
            _entity.AddLastingDamageInterval(new(_config.GetEffectIntervalByLevel(CurrentLevel.Value)));

            _entity.ProjectileType.Value = ProjectilesTypes.PoisonedArrow;
        }
    }
}
