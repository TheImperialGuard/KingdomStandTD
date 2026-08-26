using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class FirstAbilityDelayEndTriggerSystem : DelayEndTriggerSystem
    {
        public override void OnInit(Entity entity)
        {
            DelayEndEvent = entity.FirstAbilityDelayEndEvent;
            StartProcessEvent = entity.StartFirstAbilityEvent;

            Delay = entity.FirstAbilityDelayModifiedTime;
            ProcessCurrentTime = entity.FirstAbilityProcessTimerC.CurrentTime;

            ProcessTimerDisposable = ProcessCurrentTime.Subscribe(OnTimerChanged);
            StartProcessEventDisposable = StartProcessEvent.Subscribe(OnStartProcess);
        }
    }

    public class SecondAbilityDelayEndTriggerSystem : DelayEndTriggerSystem
    {
        public override void OnInit(Entity entity)
        {
            DelayEndEvent = entity.SecondAbilityDelayEndEvent;
            StartProcessEvent = entity.StartSecondAbilityEvent;

            Delay = entity.SecondAbilityDelayModifiedTime;
            ProcessCurrentTime = entity.SecondAbilityProcessTimerC.CurrentTime;

            ProcessTimerDisposable = ProcessCurrentTime.Subscribe(OnTimerChanged);
            StartProcessEventDisposable = StartProcessEvent.Subscribe(OnStartProcess);
        }
    }
}
