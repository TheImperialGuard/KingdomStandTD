using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class FirstAbilityCooldownTimerSystem : CooldownTimerSystem
    {
        public override void OnInit(Entity entity)
        {
            CurrentTime = entity.FirstAbilityCooldownTimerC.CurrentTime;
            InitialTime = entity.FirstAbilityCooldownTimerC.ModifiedTime;
            InCooldown = entity.InFirstAbilityCooldown;

            StartCooldownEvent = entity.EndFirstAbilityEvent;

            StartCooldownEventDisposable = StartCooldownEvent.Subscribe(OnStartCooldownEvent);
        }
    }

    public class SecondAbilityCooldownTimerSystem : CooldownTimerSystem
    {
        public override void OnInit(Entity entity)
        {
            CurrentTime = entity.SecondAbilityCooldownTimerC.CurrentTime;
            InitialTime = entity.SecondAbilityCooldownTimerC.ModifiedTime;
            InCooldown = entity.InSecondAbilityCooldown;

            StartCooldownEvent = entity.EndSecondAbilityEvent;

            StartCooldownEventDisposable = StartCooldownEvent.Subscribe(OnStartCooldownEvent);
        }
    }
}
