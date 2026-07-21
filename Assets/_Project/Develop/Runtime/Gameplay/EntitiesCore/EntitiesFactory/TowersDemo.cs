using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateTowerDemo(Vector3 position, TowerConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            entity.AddSelfReleaseRequested(new(false));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.SelfReleaseRequested.Value == true));

            entity
                .AddMustSelfRelease(mustSelfRelease);

            entity.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            entity.ShootingRangeZone.SetRange(config.AttackRange);
            entity.ShootingRangeZone.Show();

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
