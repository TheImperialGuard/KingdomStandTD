using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    public class TowerType : IEntityComponent
    {
        public ReactiveVariable<TowerTypes> Value;
    }

    public class TowerLevel : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
}
