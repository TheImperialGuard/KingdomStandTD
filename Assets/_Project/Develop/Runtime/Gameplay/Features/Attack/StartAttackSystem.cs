using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : StartProcessSystem
    {
        public override void OnInit(Entity entity)
        {
            StartProcessRequest = entity.StartAttackRequest;
            StartProcessEvent = entity.StartAttackEvent;

            InProcessProcess = entity.InAttackProcess;

            CanStartProcess = entity.CanStartAttack;

            StartProcessRequestDisposable = StartProcessRequest.Subscribe(OnStartProcessRequest);
        }
    }
}
