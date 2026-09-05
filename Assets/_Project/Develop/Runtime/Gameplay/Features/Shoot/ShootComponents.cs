using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class ShootPoint : IEntityComponent
    {
        public Transform Value;
    }

    public class InstantShotDirection : IEntityComponent
    {
        public ReactiveVariable<InstantShotDirectionArgs> Value;
    }

    public class InstantShootRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ProjectileSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsProjectile : IEntityComponent
    {
    }

    public class Owner : IEntityComponent
    {
        public ReactiveVariable<Entity> Value;
    }

    public class AimingPoint : IEntityComponent
    {
        public Transform Value;
    }

    public class ShootingRangeZoneComponent : IEntityComponent
    {
        public ShootingRangeZone Value;
    }

    public class BallisticTrajectory : IEntityComponent
    {
        public ReactiveVariable<TrajectoryResult> Value;
    }

    public class BallisticTrajectoryMaxHeight : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
