using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Sensors
{
    public class BoxColliderRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private BoxCollider _boxCollider;

        public override void Register(Entity entity)
        {
            entity.AddBoxCollider(_boxCollider);
        }
    }
}
