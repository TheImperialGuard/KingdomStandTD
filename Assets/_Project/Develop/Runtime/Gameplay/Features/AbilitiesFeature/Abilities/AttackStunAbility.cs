using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class AttackStunAbility : Ability, IDisposable
    {
        private readonly Entity _entity;
        private readonly AttackStunAbilityConfig _config;

        private IDisposable _currentLevelChangedDisposable;

        public AttackStunAbility(
            Entity entity,
            AttackStunAbilityConfig config,
            int currentLevel) : base(config.ID, currentLevel, config.MaxLevel)
        {
            _entity = entity;
            _config = config;
        }

        public override void Activate()
        {
            _entity.AddStunDuration(new(_config.GetStunTimeBy(CurrentLevel.Value)));
            _entity.MustStunOnAttack.Value = true;

            _currentLevelChangedDisposable = CurrentLevel.Subscribe(OnCurrentLevelChaged);
        }

        public void Dispose()
        {
            _currentLevelChangedDisposable.Dispose();
        }

        private void OnCurrentLevelChaged(int previousLevel, int newLevel) 
            => _entity.StunDuration.Value = _config.GetStunTimeBy(newLevel);
    }
}
