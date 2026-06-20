using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class DisableCollidersOnDeathRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private List<Collider> _colliders;

        public override void Register(Entity entity)
        {
            entity.AddDisableCollidersOnDeath(_colliders);
        }
    }
}
