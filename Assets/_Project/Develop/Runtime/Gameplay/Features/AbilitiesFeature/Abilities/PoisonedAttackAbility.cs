using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class PoisonedAttackAbility : Ability, IDisposable
    {
        private readonly Entity _entity;
        private readonly PoisonedAttackAbilityConfig _config;

        private IDisposable _currentLevelChangedDisposable;

        public PoisonedAttackAbility(
            Entity entity,
            PoisonedAttackAbilityConfig config,
            int currentLevel) : base(config.ID, currentLevel, config.MaxLevel)
        {
            _entity = entity;
            _config = config;
        }

        public override void Activate()
        {
            _entity.ProjectileType.Value = ProjectilesTypes.PoisonedArrow;

            SetupLastingDamage(CurrentLevel.Value);

            _currentLevelChangedDisposable = CurrentLevel.Subscribe(OnCurrentLevelChaged);
        }

        public void Dispose()
        {
            _currentLevelChangedDisposable.Dispose();
        }

        private void OnCurrentLevelChaged(int previousLevel, int newLevel) => SetupLastingDamage(newLevel);

        private void SetupLastingDamage(int level)
        {
            _entity.LastingDamage.Value = _config.GetDamageByLevel(level);
            _entity.LastingDamageInitialTime.Value = _config.GetEffectTimeByLevel(level);
            _entity.LastingDamageInterval.Value = _config.GetEffectIntervalByLevel(level);
        }
    }
}
