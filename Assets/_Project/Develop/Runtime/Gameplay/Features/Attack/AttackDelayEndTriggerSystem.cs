using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackDelayEndTriggerSystem : DelayEndTriggerSystem
    {
        public override void OnInit(Entity entity)
        {
            DelayEndEvent = entity.AttackDelayEndEvent;
            StartProcessEvent = entity.StartAttackEvent;

            Delay = entity.AttackDelayModifiedTime;
            ProcessCurrentTime = entity.AttackProcessCurrentTime;

            ProcessTimerDisposable = ProcessCurrentTime.Subscribe(OnTimerChanged);
            StartProcessEventDisposable = StartProcessEvent.Subscribe(OnStartProcess);
        }
    }
}
