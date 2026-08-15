using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class StatusesApplierSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private readonly TimerServiceFactory _timerServiceFactory;

        private Entity _entity;
        private StatusesList _statuses;

        private Dictionary<Status, TimerService> _activeStatuses = new();

        public StatusesApplierSystem(TimerServiceFactory timerServiceFactory)
        {
            _timerServiceFactory = timerServiceFactory;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _statuses = entity.Statuses;

            _statuses.Added += OnStatusAdded;
            _statuses.Removed += OnStatusRemoved;
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (KeyValuePair<Status, TimerService> activeStatus in _activeStatuses)
            {
                if (activeStatus.Value.IsOver)
                {
                    Debug.Log($"{_entity.Transform.gameObject.name} получил урон от отравления, было хп: {_entity.CurrentHealth.Value}");
                    activeStatus.Key.ApplyEffect();
                    Debug.Log($"Осталось хп: {_entity.CurrentHealth.Value}");
                    activeStatus.Value.Restart();
                }
            }
        }

        public void OnDispose()
        {
            foreach (TimerService timerService in _activeStatuses.Values)
                timerService.Dispose();

            _statuses.Added -= OnStatusAdded;
            _statuses.Removed -= OnStatusRemoved;
        }

        private void OnStatusAdded(Status status)
        {
            if (_activeStatuses.ContainsKey(status))
                return;

            TimerService timer = _timerServiceFactory.Create(status.Interval.Value);

            _activeStatuses.Add(status, timer);

            timer.Restart();
        }

        private void OnStatusRemoved(Status status)
        {
            _activeStatuses[status].Dispose();
            _activeStatuses.Remove(status);
        }
    }
}
