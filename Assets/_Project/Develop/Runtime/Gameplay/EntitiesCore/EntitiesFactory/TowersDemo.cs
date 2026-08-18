using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateTowerDemo(Vector3 position, ShootingTowerConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            entity
                .AddInstantShootRange(new(config.AttackRange))
                .AddSelfReleaseRequested(new(false));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.SelfReleaseRequested.Value == true));

            entity
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                .AddSystem(new RangeZoneRadiusSyncSystem());

            entity.ShootingRangeZone.Show();

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
