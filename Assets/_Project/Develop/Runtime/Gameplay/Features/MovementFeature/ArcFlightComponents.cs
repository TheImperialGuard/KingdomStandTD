using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class ArcFlightStartPoint : IEntityComponent
    {
        public ReactiveVariable<Vector3> Value;
    }

    public class ArcFlightEndPoint : IEntityComponent
    {
        public ReactiveVariable<Vector3> Value;
    }

    public class ArcFlightHeight : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ArcFlightDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ArcFlightCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsArcFlightFinished : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
