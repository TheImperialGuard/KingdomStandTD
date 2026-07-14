using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class AimingPointEntityRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private Transform _aimingPoint;

        public override void Register(Entity entity)
        {
            entity.AddAimingPoint(_aimingPoint);
        }
    }
}
