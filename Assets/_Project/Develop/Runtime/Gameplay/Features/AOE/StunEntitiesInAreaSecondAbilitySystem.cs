using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class StunEntitiesInAreaSecondAbilitySystem : IInitializableSystem, IDisposableSystem
    {
        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;
        private readonly StatusesFactory _statusesFactory;

        private Entity _source;

        private ReactiveEvent _secondAbilityDelayEndEvent;
        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<float> _stunDuration;
        private ReactiveVariable<float> _areaRadius;

        private IDisposable _abilityDelayDisposable;

        public StunEntitiesInAreaSecondAbilitySystem(
            AreaEntitiesDetectorService areaEntitiesDetectorService, 
            StatusesFactory statusesFactory)
        {
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
            _statusesFactory = statusesFactory;
        }

        public void OnInit(Entity entity)
        {
            _source = entity;

            _secondAbilityDelayEndEvent = entity.SecondAbilityDelayEndEvent;
            _currentTarget = entity.CurrentTarget;
            _stunDuration = entity.StunDuration;
            _areaRadius = entity.AreaEffectRadius;

            _abilityDelayDisposable = _secondAbilityDelayEndEvent.Subscribe(OnAbilityDelayEnd);
        }

        public void OnDispose()
        {
            _abilityDelayDisposable.Dispose();
        }

        private void OnAbilityDelayEnd()
        {
            List<Entity> entities = _areaEntitiesDetectorService.GetEntitiesInArea(
                _currentTarget.Value.Transform.position,
                _areaRadius.Value);

            foreach (Entity entity in entities)
            {
                if (entity.TryGetIsStunned(out ReactiveVariable<bool> isStunned) == false)
                    continue;

                Stun(entity);
            }
        }

        private void Stun(Entity entity)
        {
            Status status = _statusesFactory.CreateFor(
                entity, 
                StatusesTypes.Stun, 
                _source, 
                _stunDuration.Value, 
                _stunDuration.Value);

            entity.Statuses.AddElement(status);
        }
    }
}
