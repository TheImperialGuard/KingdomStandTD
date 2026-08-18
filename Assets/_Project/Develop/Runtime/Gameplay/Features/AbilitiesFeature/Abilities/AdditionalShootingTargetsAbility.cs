using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class AdditionalShootingTargetsAbility : Ability, IDisposable
    {
        private const int BaseTargetsCount = 1;

        private AdditionalShootingTargetsAbilityConfig _config;
        private Entity _entity;

        private IDisposable _currentLevelChangedDisposable;

        public AdditionalShootingTargetsAbility(
            AdditionalShootingTargetsAbilityConfig config,
            Entity entity,
            int currentLevel) 
            : base(config.ID, currentLevel, config.MaxLevel)
        {
            _config = config;
            _entity = entity;
        }

        public override void Activate()
        {
            SetMaxTargetsBy(CurrentLevel.Value);

            _currentLevelChangedDisposable = CurrentLevel.Subscribe(OnCurrentLevelChaged);
        }

        public void Dispose()
        {
            _currentLevelChangedDisposable.Dispose();
        }

        private void OnCurrentLevelChaged(int previousLevel, int newLevel)
        {
            SetMaxTargetsBy(newLevel);
        }

        private void SetMaxTargetsBy(int level)
        {
            int additionalTargetsCount = _config.GetAdditionsTargetsBy(level);

            _entity.MaxTargets.Value = BaseTargetsCount + additionalTargetsCount;
        }
    }
}
