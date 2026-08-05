using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation
{
    public class CurrentWaypoint : IEntityComponent
    {
        public ReactiveVariable<Waypoint> Value;
    }

    public class Waypoints : IEntityComponent
    {
        public List<Waypoint> Value;
    }

    public class ReachedWaypoints : IEntityComponent
    {
        public List<Waypoint> Value;
    }

    public class IsPathFinished : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class DamageOnFinishPath : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class WaypointsOffset : IEntityComponent
    {
        public ReactiveVariable<Vector3> Value;
    }
}
