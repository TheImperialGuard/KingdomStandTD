using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class StunEnemiesInAreaOnDeathSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;
        private readonly StatusesFactory _statusesFactory;

        private ReactiveVariable<float> _stunDuration;
        private ReactiveVariable<float> _areaRadius;

        private Entity _source;
        private Transform _deathPoint;

        private ReactiveVariable<bool> _isDead;

        private IDisposable _isDeadDisposable;

        public StunEnemiesInAreaOnDeathSystem(
            AreaEntitiesDetectorService areaEntitiesDetectorService, 
            StatusesFactory statusesFactory)
        {
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
            _statusesFactory = statusesFactory;
        }

        public void OnInit(Entity entity)
        {
            _stunDuration = entity.StunDuration;
            _areaRadius = entity.AreaEffectRadius;

            _source = entity;
            _deathPoint = entity.Transform;

            _isDead = entity.IsDead;

            _isDeadDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        public void OnDispose()
        {
            _isDeadDisposable.Dispose();
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead == true)
                StunEnemiesInArea();
        }

        private void StunEnemiesInArea()
        {
            List<Entity> entities = _areaEntitiesDetectorService.GetEntitiesInArea(
                _deathPoint.position,
                _areaRadius.Value);

            foreach (Entity entityInArea in entities)
            {
                if (entityInArea.TryGetIsStunned(out ReactiveVariable<bool> isStunned) == false)
                    continue;

                if (isStunned.Value == true)
                    continue;

                Stun(entityInArea);
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
