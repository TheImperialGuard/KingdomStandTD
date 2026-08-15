using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class StatusesLifeCycleSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly TimerServiceFactory _timerServiceFactory;

        private StatusesList _statuses;

        private Dictionary<Status, TimerService> _activeStatuses = new();

        private Dictionary<TimerService, IDisposable> _timerCooldownEndedDisposablesWithTimers = new();

        public StatusesLifeCycleSystem(TimerServiceFactory timerServiceFactory)
        {
            _timerServiceFactory = timerServiceFactory;
        }

        public void OnInit(Entity entity)
        {
            _statuses = entity.Statuses;

            _statuses.Added += OnStatusAdded;
            _statuses.Removed += OnStatusRemoved;
        }

        public void OnDispose()
        {
            foreach (TimerService timerService in _activeStatuses.Values)
                timerService.Dispose();

            foreach (IDisposable disposable in _timerCooldownEndedDisposablesWithTimers.Values)
                disposable.Dispose();

            _statuses.Added -= OnStatusAdded;
            _statuses.Removed -= OnStatusRemoved;

            _activeStatuses.Clear();
            _timerCooldownEndedDisposablesWithTimers.Clear();
        }

        private void OnStatusAdded(Status status)
        {
            if (_activeStatuses.ContainsKey(status))
            {
                _activeStatuses[status].Restart();
                return;
            }

            TimerService timer = _timerServiceFactory.Create(status.InitialTime.Value);

            _timerCooldownEndedDisposablesWithTimers.Add(timer, timer.CooldownEndedWithTimer.Subscribe(OnTimerCooldownEnded));

            _activeStatuses.Add(status, timer);

            timer.Restart();
        }

        private void OnTimerCooldownEnded(TimerService timer)
        {
            Status status = _activeStatuses.First(kvp => kvp.Value == timer).Key;

            _statuses.RemoveElement(status);
        }

        private void OnStatusRemoved(Status status)
        {
            TimerService timer = _activeStatuses[status];

            _timerCooldownEndedDisposablesWithTimers[timer].Dispose();
            _timerCooldownEndedDisposablesWithTimers.Remove(timer);

            timer.Dispose();

            _activeStatuses.Remove(status);
        }
    }
}
