using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public interface IStatusEffect
    {
        void OnAdd(Entity entity);
        void Apply(Entity entity);
        void OnRemove(Entity entity);
    }
}
