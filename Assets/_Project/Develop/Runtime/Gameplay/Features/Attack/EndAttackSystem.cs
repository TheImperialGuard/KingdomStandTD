using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class EndAttackSystem : EndProcessSystem
    {
        public override void OnInit(Entity entity)
        {
            EndProcessEvent = entity.EndAttackEvent;

            InProcess = entity.InAttackProcess;

            ProcessInitialTime = entity.AttackProcessModifiedTime;
            ProcessCurrentTime = entity.AttackProcessCurrentTime;

            CurrentTimeDisposable = ProcessCurrentTime.Subscribe(OnCurrentTimeChanged);
        }
    }
}
