using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackCooldownTimerSystem : CooldownTimerSystem
    {
        public override void OnInit(Entity entity)
        {
            CurrentTime = entity.AttackCooldownCurrentTime;
            InitialTime = entity.AttackCooldownModifiedTime;
            InCooldown = entity.InAttackCooldown;

            StartCooldownEvent = entity.EndAttackEvent;

            StartCooldownEventDisposable = StartCooldownEvent.Subscribe(OnStartCooldownEvent);
        }
    }
}
