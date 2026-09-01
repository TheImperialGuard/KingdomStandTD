using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class StartFirstAbilitySystem : StartProcessSystem
    {
        public override void OnInit(Entity entity)
        {
            StartProcessRequest = entity.StartFirstAbilityRequest;
            StartProcessEvent = entity.StartFirstAbilityEvent;

            InProcessProcess = entity.InFirstAbilityProcess;

            CanStartProcess = entity.CanStartFirstAbility;

            StartProcessRequestDisposable = StartProcessRequest.Subscribe(OnStartProcessRequest);
        }
    }

    public class StartSecondAbilitySystem : StartProcessSystem
    {
        public override void OnInit(Entity entity)
        {
            StartProcessRequest = entity.StartSecondAbilityRequest;
            StartProcessEvent = entity.StartSecondAbilityEvent;

            InProcessProcess = entity.InSecondAbilityProcess;

            CanStartProcess = entity.CanStartSecondAbility;

            StartProcessRequestDisposable = StartProcessRequest.Subscribe(OnStartProcessRequest);
        }
    }
}
