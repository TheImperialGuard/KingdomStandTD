using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class ProjectileFragmentsAbility : Ability, IDisposable
    {
        private readonly Entity _entity;
        private readonly ProjectileFragmentsAbilityConfig _config;

        private IDisposable _currentLevelChangedDisposable;

        public ProjectileFragmentsAbility(
            Entity entity,
            ProjectileFragmentsAbilityConfig config,
            int currentLevel) : base(config.ID, currentLevel, config.MaxLevel)
        {
            _entity = entity;
            _config = config;
        }

        public override void Activate()
        {
            _entity.AddFragmentsDamage(new(_config.GetFragmentsDamageBy(CurrentLevel.Value)));
            _entity.AddFragmentsCount(new(_config.FragmentsCount));
            _entity.MustCreateFragmentsOnProjectileDeath.Value = true;

            _currentLevelChangedDisposable = CurrentLevel.Subscribe(OnCurrentLevelChaged);
        }

        public void Dispose()
        {
            _currentLevelChangedDisposable.Dispose();
        }

        private void OnCurrentLevelChaged(int previousLevel, int newLevel)
            => _entity.FragmentsDamage.Value = _config.GetFragmentsDamageBy(newLevel);
    }
}
