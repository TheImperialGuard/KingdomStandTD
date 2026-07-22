using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.Interactables;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateTowerPlaceholder(Vector3 position, TowerPlaceholderConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            IInteractAction interactAction = _interactiveActionsFactory.CreateBuildTowerAction(entity);

            entity
                .AddIsInteractable()
                .AddInteractRequest()
                .AddInteractEvent()
                .AddInteractiveAction(new(interactAction))
                .AddSelfReleaseRequested(new(false));
                                                             
            ICompositeCondition canInteract = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.SelfReleaseRequested.Value == true));

            entity
                .AddCanInteract(canInteract)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new InteractSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
