using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackProcessTimerSystem : ProcessTimerSystem
    {
        public override void OnInit(Entity entity)
        {
            CurrentTime = entity.AttackProcessCurrentTime;
            InProcess = entity.InAttackProcess;
            StartProcessEvent = entity.StartAttackEvent;

            StartProcessEventDisposable = StartProcessEvent.Subscribe(OnStartProcessEvent);
        }
    }
}
