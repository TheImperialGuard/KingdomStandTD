using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class ShootingRangeZoneEntityRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private ShootingRangeZone _zone;

        public override void Register(Entity entity)
        {
            entity.AddShootingRangeZone(_zone);
        }
    }
}
