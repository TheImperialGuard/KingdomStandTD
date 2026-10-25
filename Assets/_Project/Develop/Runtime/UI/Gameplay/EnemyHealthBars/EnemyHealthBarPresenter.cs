using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.EnemyHealthBars
{
    // Полоска одного врага: здоровье, значки статусов и скрытие при смерти. Элемент приходит из пула, поэтому
    // при старте показывается и выставляет всё заново
    public class EnemyHealthBarPresenter : IPresenter
    {
        private readonly EnemyHealthBarView _view;
        private readonly Entity _enemy;

        private readonly List<IDisposable> _disposables = new();

        private IReadOnlyVariable<float> _currentHealth;
        private IReadOnlyVariable<float> _maxHealth;
        private StatusesList _statuses;

        public EnemyHealthBarPresenter(EnemyHealthBarView view, Entity enemy)
        {
            _view = view;
            _enemy = enemy;
        }

        public void Initialize()
        {
            _currentHealth = _enemy.CurrentHealth;
            _maxHealth = _enemy.MaxHealth;

            _disposables.Add(_currentHealth.Subscribe(OnHealthChanged));
            _disposables.Add(_maxHealth.Subscribe(OnHealthChanged));
            _disposables.Add(_enemy.IsDead.Subscribe(OnIsDeadChanged));

            _view.Show();
            _view.HideStatuses();

            UpdateHealth();

            if (_enemy.TryGetStatuses(out _statuses))
            {
                _statuses.Added += OnStatusesChanged;
                _statuses.Removed += OnStatusesChanged;

                foreach (Status status in _statuses.Elements)
                    _view.SetStatusShown(status.Type, true);
            }
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();

            if (_statuses != null)
            {
                _statuses.Added -= OnStatusesChanged;
                _statuses.Removed -= OnStatusesChanged;
                _statuses = null;
            }
        }

        private void OnHealthChanged(float oldValue, float newValue) => UpdateHealth();

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead == true)
                _view.Hide();
        }

        private void OnStatusesChanged(Status status) => _view.SetStatusShown(status.Type, _statuses.Has(status.Type));

        private void UpdateHealth()
        {
            float value = _currentHealth.Value / _maxHealth.Value;

            _view.SetValue(value);
        }
    }
}
