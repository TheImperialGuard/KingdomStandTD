using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning
{
    public class GoldOnDeath : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
}
