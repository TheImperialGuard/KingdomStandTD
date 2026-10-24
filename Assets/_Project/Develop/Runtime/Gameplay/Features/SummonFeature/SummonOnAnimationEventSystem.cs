using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SummonFeature
{
    // Поднимает строй призванных по событию Summon в клипе каста. Призыв держится на этом событии: если переименовать или
    // удалить его в клипе или поставить аниматору отсечение Cull Completely, призыва не будет. При Cull Update Transforms
    // событие приходит и тогда, когда некромант за кадром.
    // События анимации приходят вне прохода EntitiesLifeContext.Update, поэтому создавать сущности здесь безопасно.
    public class SummonOnAnimationEventSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly EnemiesFactory _enemiesFactory;

        private SummonAnimationEventReceiver _receiver;

        private ReactiveVariable<bool> _isDead;
        private ReactiveVariable<bool> _isPathFinished;

        private Transform _transform;
        private List<Waypoint> _waypoints;
        private List<Waypoint> _reachedWaypoints;
        private ReactiveVariable<Vector3> _waypointsOffset;

        private MeleeConfig _summonedConfig;
        private ReactiveVariable<int> _count;
        private ReactiveVariable<float> _rowSpacing;
        private ReactiveVariable<float> _columnSpacing;

        public SummonOnAnimationEventSystem(EnemiesFactory enemiesFactory)
        {
            _enemiesFactory = enemiesFactory;
        }

        public void OnInit(Entity entity)
        {
            _receiver = entity.SummonAnimationEventReceiver;

            _isDead = entity.IsDead;
            _isPathFinished = entity.IsPathFinished;

            _transform = entity.Transform;
            _waypoints = entity.Waypoints;
            _reachedWaypoints = entity.ReachedWaypoints;
            _waypointsOffset = entity.WaypointsOffset;

            _summonedConfig = entity.SummonedEnemyConfig;
            _count = entity.SummonCount;
            _rowSpacing = entity.SummonRowSpacing;
            _columnSpacing = entity.SummonColumnSpacing;

            _receiver.Summoned += OnSummoned;
        }

        public void OnDispose()
        {
            _receiver.Summoned -= OnSummoned;
        }

        private static Vector3 GetLanePoint(Waypoint waypoint, Vector3 waypointsOffset)
        {
            Transform waypointTransform = waypoint.transform;

            return waypointTransform.position + waypointTransform.TransformVector(waypointsOffset);
        }

        private void OnSummoned()
        {
            if (_isDead.Value || _isPathFinished.Value)
                return;

            int nextWaypointIndex = _reachedWaypoints.Count;

            SummonFormationLayout layout = new SummonFormationLayout(
                _transform.position,
                GetLanePoint(_waypoints[nextWaypointIndex], _waypointsOffset.Value),
                GetPathPoints(),
                nextWaypointIndex,
                _waypointsOffset.Value.x,
                _rowSpacing.Value,
                _columnSpacing.Value);

            foreach (SummonFormationCell cell in layout.GetCells(_count.Value))
                Summon(cell);
        }

        private List<Vector3> GetPathPoints()
        {
            List<Vector3> pathPoints = new List<Vector3>(_waypoints.Count);

            foreach (Waypoint waypoint in _waypoints)
                pathPoints.Add(waypoint.transform.position);

            return pathPoints;
        }

        // Призванный идёт по пути призывающего с вейпоинта перед своей клеткой и сразу смотрит на него.
        private void Summon(SummonFormationCell cell)
        {
            List<Waypoint> path = _waypoints.GetRange(cell.FirstWaypointIndex, _waypoints.Count - cell.FirstWaypointIndex);
            Vector3 waypointsOffset = PathLaneOffsets.ToLocalOffset(cell.Lane);

            Vector3 lookDirection = Vector3.ProjectOnPlane(GetLanePoint(path[0], waypointsOffset) - cell.Position, Vector3.up);

            _enemiesFactory.Create(cell.Position, _summonedConfig, path, waypointsOffset, lookDirection);
        }
    }
}
