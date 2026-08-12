using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

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

    public class SubTowerParent : IEntityComponent
    {
        public Entity Value;
    }

    public class IsSubTower : IEntityComponent
    {
    }

    public class SubTowerCreator : IEntityComponent
    {
        public Func<Entity, Entity> Value;
    }
}
