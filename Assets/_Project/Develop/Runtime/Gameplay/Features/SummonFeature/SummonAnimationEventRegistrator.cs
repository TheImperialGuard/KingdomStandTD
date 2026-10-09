using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SummonFeature
{
    // Отдаёт сущности приёмник событий клипа призыва — он стоит на объекте с Animator.
    public class SummonAnimationEventRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private SummonAnimationEventReceiver _receiver;

        public override void Register(Entity entity)
        {
            entity.AddSummonAnimationEventReceiver(_receiver);
        }
    }
}
