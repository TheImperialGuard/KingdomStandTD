using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesHelper
    {
        public static bool TryTakeDamageFrom(Entity source, Entity target, float damage)
        {
            if (target.TryGetTakeDamageRequest(out ReactiveEvent<float> takeDamageRequest) == false)
                return false;

            if (IsSameTeam(source, target))
                return false;

            takeDamageRequest.Invoke(damage);

            return true;
        }

        public static bool IsSameTeam(Entity firstEntity, Entity secondEntity)
        {
            if (firstEntity.TryGetTeam(out ReactiveVariable<Teams> firstTeam)
                && secondEntity.TryGetTeam(out ReactiveVariable<Teams> secondTeam))
            {
                return firstTeam.Value == secondTeam.Value;
            }

            return false;
        }
    }
}
