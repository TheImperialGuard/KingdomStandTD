using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class StunStatusEffect : IStatusEffect
    {
        public void Apply(Entity entity)
        {
        }

        public void OnAdd(Entity entity)
        {
            entity.IsStunned.Value = true;
        }

        public void OnRemove(Entity entity)
        {
            entity.IsStunned.Value = false;
        }
    }
}
