using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class DealAreaDamageOnDeathSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;

        private ReactiveVariable<float> _damage;
        private ReactiveVariable<float> _areaRadius;

        private Entity _source;
        private Transform _deathPoint;

        private ReactiveVariable<bool> _isDead;

        private IDisposable _isDeadDisposable;

        public DealAreaDamageOnDeathSystem(AreaEntitiesDetectorService areaEntitiesDetectorService)
        {
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
        }

        public void OnInit(Entity entity)
        {
            _damage = entity.InstantAttackDamage;
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
                DealAreaDamage();
        }

        private void DealAreaDamage()
        {
            List<Entity> entities = _areaEntitiesDetectorService.GetEntitiesInArea(
                _deathPoint.position,
                _areaRadius.Value);

            foreach (Entity entityInArea in entities)
            {
                EntitiesHelper.TryTakeDamageFrom(_source, entityInArea, _damage.Value);
            }
        }
    }
}
