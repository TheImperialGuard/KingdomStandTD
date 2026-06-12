using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common
{
    public class RigidbodyComponent : IEntityComponent
    {
        public Rigidbody Value;
    }

    public class TransformComponent : IEntityComponent
    {
        public Transform Value;
    }
}
