using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature
{
    // Цель выбирается в начале каста, снежок вылетает по событию ReleaseSnowball в клипе броска. Бросок держится на этом
    // событии: если переименовать или удалить его в клипе или поставить аниматору отсечение Cull Completely, броска не будет.
    // События анимации приходят вне прохода EntitiesLifeContext.Update, поэтому создавать снежок здесь безопасно.
    public class SnowballThrowSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly BuiltTowersService _builtTowersService;
        private readonly ProjectilesFactory _projectilesFactory;

        private readonly List<Entity> _towersInRange = new();
        private readonly List<Entity> _unfrozenTowers = new();

        private SnowballThrowAnimationEventReceiver _receiver;
        private Transform _transform;

        private ReactiveVariable<bool> _isDead;
        private ReactiveVariable<bool> _isPathFinished;

        private ReactiveVariable<Entity> _target;
        private ReactiveVariable<float> _range;
        private ReactiveVariable<float> _speed;
        private ReactiveVariable<float> _arcHeight;
        private ReactiveVariable<float> _freezeDuration;
        private ReactiveVariable<string> _prefabPath;

        private IDisposable _startAbilityDisposable;
        private IDisposable _endAbilityDisposable;

        public SnowballThrowSystem(BuiltTowersService builtTowersService, ProjectilesFactory projectilesFactory)
        {
            _builtTowersService = builtTowersService;
            _projectilesFactory = projectilesFactory;
        }

        public void OnInit(Entity entity)
        {
            _receiver = entity.SnowballThrowAnimationEventReceiver;
            _transform = entity.Transform;

            _isDead = entity.IsDead;
            _isPathFinished = entity.IsPathFinished;

            _target = entity.SnowballThrowTarget;
            _range = entity.SnowballThrowRange;
            _speed = entity.SnowballSpeed;
            _arcHeight = entity.SnowballArcHeight;
            _freezeDuration = entity.FreezeDuration;
            _prefabPath = entity.SnowballPrefabPath;

            _startAbilityDisposable = entity.StartFirstAbilityEvent.Subscribe(OnAbilityStarted);
            _endAbilityDisposable = entity.EndFirstAbilityEvent.Subscribe(OnAbilityEnded);
            _receiver.SnowballReleased += OnSnowballReleased;
        }

        public void OnDispose()
        {
            _startAbilityDisposable?.Dispose();
            _endAbilityDisposable?.Dispose();
            _receiver.SnowballReleased -= OnSnowballReleased;
        }

        private static bool IsGone(Entity tower) => tower.IsInit == false || tower.SelfReleaseRequested.Value;

        // Точка попадания — верх коллайдера башни над его центром.
        private static Vector3 GetHitPoint(Entity tower)
        {
            Bounds bounds = tower.BoxCollider.bounds;

            return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        }

        private void OnAbilityStarted() => _target.Value = SelectTarget();

        private void OnAbilityEnded() => _target.Value = null;

        private void OnSnowballReleased()
        {
            if (_isDead.Value || _isPathFinished.Value)
                return;

            // Башню за время замаха могли продать или улучшить — выбираем заново
            if (_target.Value == null || IsGone(_target.Value))
                _target.Value = SelectTarget();

            Entity target = _target.Value;

            // Башен рядом не осталось — бросок вхолостую
            if (target == null)
                return;

            _projectilesFactory.CreateSnowball(
                _receiver.ReleasePoint.position,
                GetHitPoint(target),
                target,
                _speed.Value,
                _arcHeight.Value,
                _freezeDuration.Value,
                _prefabPath.Value);
        }

        // Незамороженные башни в приоритете: случайная среди них, а если все заморожены — среди всех.
        private Entity SelectTarget()
        {
            _builtTowersService.GetTowersInRange(_transform.position, _range.Value, _towersInRange);

            if (_towersInRange.Count == 0)
                return null;

            _unfrozenTowers.Clear();

            foreach (Entity tower in _towersInRange)
            {
                if (tower.IsFrozen.Value == false)
                    _unfrozenTowers.Add(tower);
            }

            List<Entity> candidates = _unfrozenTowers.Count > 0 ? _unfrozenTowers : _towersInRange;

            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }
    }
}
