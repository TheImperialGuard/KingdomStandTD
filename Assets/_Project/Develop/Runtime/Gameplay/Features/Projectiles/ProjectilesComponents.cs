using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles
{
    public class ProjectileType : IEntityComponent
    {
        public ReactiveVariable<ProjectilesTypes> Value;
    }
}
