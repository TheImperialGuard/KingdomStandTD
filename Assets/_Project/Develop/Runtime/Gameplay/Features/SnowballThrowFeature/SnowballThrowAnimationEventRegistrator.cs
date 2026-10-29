using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature
{
    // Отдаёт сущности приёмник событий клипа броска — он стоит на объекте с Animator.
    public class SnowballThrowAnimationEventRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private SnowballThrowAnimationEventReceiver _receiver;

        public override void Register(Entity entity)
        {
            entity.AddSnowballThrowAnimationEventReceiver(_receiver);
        }
    }
}
