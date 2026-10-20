using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Raycast
{
    public static class RaycastHitExtensions
    {
        // Коллайдер сущности стоит на корне её префаба, там же, где регистратор Transform.
        public static bool IsOn(this RaycastHit hit, Entity entity)
            => hit.collider != null && hit.collider.transform == entity.Transform;
    }
}
