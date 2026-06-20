using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class SelfReleaseSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public SelfReleaseSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        private Entity _entity;

        private ICompositeCondition _mustSelfRelease;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _mustSelfRelease = entity.MustSelfRelease;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mustSelfRelease.Evaluate())
                _entitiesLifeContext.Release(_entity);
        }
    }
}
