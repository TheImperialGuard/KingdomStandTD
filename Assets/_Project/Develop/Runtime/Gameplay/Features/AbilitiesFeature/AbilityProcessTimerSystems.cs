using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class FirstAbilityProcessTimerSystem : ProcessTimerSystem
    {
        public override void OnInit(Entity entity)
        {
            CurrentTime = entity.FirstAbilityProcessTimerC.CurrentTime;
            InProcess = entity.InFirstAbilityProcess;
            StartProcessEvent = entity.StartFirstAbilityEvent;

            StartProcessEventDisposable = StartProcessEvent.Subscribe(OnStartProcessEvent);
        }
    }

    public class SecondAbilityProcessTimerSystem : ProcessTimerSystem
    {
        public override void OnInit(Entity entity)
        {
            CurrentTime = entity.SecondAbilityProcessTimerC.CurrentTime;
            InProcess = entity.InSecondAbilityProcess;
            StartProcessEvent = entity.StartSecondAbilityEvent;

            StartProcessEventDisposable = StartProcessEvent.Subscribe(OnStartProcessEvent);
        }
    }
}
