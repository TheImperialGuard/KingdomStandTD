using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class EndFirstAbilitySystems : EndProcessSystem
    {
        public override void OnInit(Entity entity)
        {
            EndProcessEvent = entity.EndFirstAbilityEvent;

            InProcess = entity.InFirstAbilityProcess;

            ProcessInitialTime = entity.FirstAbilityProcessTimerC.ModifiedTime;
            ProcessCurrentTime = entity.FirstAbilityProcessTimerC.CurrentTime;

            CurrentTimeDisposable = ProcessCurrentTime.Subscribe(OnCurrentTimeChanged);
        }
    }

    public class EndSecondAbilitySystems : EndProcessSystem
    {
        public override void OnInit(Entity entity)
        {
            EndProcessEvent = entity.EndSecondAbilityEvent;

            InProcess = entity.InSecondAbilityProcess;

            ProcessInitialTime = entity.SecondAbilityProcessTimerC.ModifiedTime;
            ProcessCurrentTime = entity.SecondAbilityProcessTimerC.CurrentTime;

            CurrentTimeDisposable = ProcessCurrentTime.Subscribe(OnCurrentTimeChanged);
        }
    }
}
