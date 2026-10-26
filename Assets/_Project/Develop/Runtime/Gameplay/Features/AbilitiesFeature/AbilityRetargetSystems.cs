using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class FirstAbilityRetargetSystem : AbilityRetargetSystem
    {
        public FirstAbilityRetargetSystem(TargetCandidatesService targetCandidatesService) : base(targetCandidatesService)
        {
        }

        public override void OnInit(Entity entity)
        {
            CurrentTarget = entity.CurrentTarget;
            TargetSelector = entity.TargetSelector;

            InProcess = entity.InFirstAbilityProcess;
            ProcessCurrentTime = entity.FirstAbilityProcessTimerC.CurrentTime;
            Delay = entity.FirstAbilityDelayModifiedTime;

            CanceledEvent = entity.FirstAbilityCanceledEvent;
        }
    }

    public class SecondAbilityRetargetSystem : AbilityRetargetSystem
    {
        public SecondAbilityRetargetSystem(TargetCandidatesService targetCandidatesService) : base(targetCandidatesService)
        {
        }

        public override void OnInit(Entity entity)
        {
            CurrentTarget = entity.CurrentTarget;
            TargetSelector = entity.TargetSelector;

            InProcess = entity.InSecondAbilityProcess;
            ProcessCurrentTime = entity.SecondAbilityProcessTimerC.CurrentTime;
            Delay = entity.SecondAbilityDelayModifiedTime;

            CanceledEvent = entity.SecondAbilityCanceledEvent;
        }
    }
}
