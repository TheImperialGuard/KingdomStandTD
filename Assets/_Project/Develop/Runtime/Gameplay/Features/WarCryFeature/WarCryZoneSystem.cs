using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WarCryFeature
{
    // Пока вожак кричит, ускоряет союзников нужного вида в радиусе вокруг себя. Зона проверяется не каждый кадр:
    // повторное наложение продлевает статус и перезапускает его таймер.
    public class WarCryZoneSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float ZoneCheckInterval = 0.1f;

        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;
        private readonly StatusesFactory _statusesFactory;

        private Entity _source;
        private Transform _transform;

        private ReactiveVariable<bool> _inProcess;
        private ReactiveVariable<bool> _isDead;

        private ReactiveVariable<float> _radius;
        private ReactiveVariable<float> _speedBoostDuration;
        private List<EnemyKinds> _targetKinds;

        private float _timeToZoneCheck;

        public WarCryZoneSystem(
            AreaEntitiesDetectorService areaEntitiesDetectorService,
            StatusesFactory statusesFactory)
        {
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
            _statusesFactory = statusesFactory;
        }

        public void OnInit(Entity entity)
        {
            _source = entity;
            _transform = entity.Transform;

            _inProcess = entity.InFirstAbilityProcess;
            _isDead = entity.IsDead;

            _radius = entity.AreaEffectRadius;
            _speedBoostDuration = entity.SpeedBoostDuration;
            _targetKinds = entity.WarCryTargetKinds;
        }

        public void OnUpdate(float deltaTime)
        {
            // Вне крика таймер обнулён, поэтому зона проверяется уже в первый кадр следующего крика.
            if (_inProcess.Value == false || _isDead.Value)
            {
                _timeToZoneCheck = 0;
                return;
            }

            _timeToZoneCheck -= deltaTime;

            if (_timeToZoneCheck > 0)
                return;

            _timeToZoneCheck = ZoneCheckInterval;

            BoostTargetsInZone();
        }

        private void BoostTargetsInZone()
        {
            List<Entity> entitiesInZone = _areaEntitiesDetectorService.GetEntitiesInArea(
                _transform.position,
                _radius.Value);

            foreach (Entity entityInZone in entitiesInZone)
            {
                if (IsTarget(entityInZone))
                    Boost(entityInZone);
            }
        }

        private bool IsTarget(Entity entity)
        {
            if (entity == _source)
                return false;

            if (EntitiesHelper.IsSameTeam(_source, entity) == false)
                return false;

            if (entity.TryGetEnemyKind(out ReactiveVariable<EnemyKinds> kind) == false)
                return false;

            if (_targetKinds.Contains(kind.Value) == false)
                return false;

            if (entity.TryGetIsDead(out ReactiveVariable<bool> isDead) == false)
                return false;

            return isDead.Value == false;
        }

        private void Boost(Entity target)
        {
            Status status = _statusesFactory.CreateFor(
                target,
                StatusesTypes.SpeedBoost,
                _source,
                _speedBoostDuration.Value,
                _speedBoostDuration.Value);

            EntitiesHelper.TryInjectStatusTo(target, status);
        }
    }
}
