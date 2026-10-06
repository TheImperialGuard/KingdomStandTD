using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.Features.SoundEffects;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class OneShotAbilitySoundEffectView : SoundEffectViewBase
    {
        private Entity _entity;

        private IDisposable _abilityDelayEndEventDisposable;

        protected override void Subscribe(Entity entity)
        {
            _entity = entity;

            _entity.Abilities.Added += OnAbilityAdded;
        }

        protected override void Unsubscribe()
        {
            _abilityDelayEndEventDisposable?.Dispose();
            _entity.Abilities.Added -= OnAbilityAdded;
        }

        private void OnAbilityAdded(Ability ability)
        {
            if (ability is OneShotAttackAbility == false)
                return;

            _abilityDelayEndEventDisposable = _entity.FirstAbilityDelayEndEvent.Subscribe(PlaySound);
        }
    }
}
