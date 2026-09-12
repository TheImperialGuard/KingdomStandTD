using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class CurrentHealthView : EntityView
    {
        [SerializeField] private Bar _healthBar;

        private IReadOnlyVariable<float> _currentHealth;
        private IReadOnlyVariable<float> _maxHealth;

        private IDisposable _curentHealthChangedDisposable;
        private IDisposable _maxHealthChangedDisposable;

        private IReadOnlyVariable<bool> _isDead;

        private IDisposable _isDeadChangedDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _curentHealthChangedDisposable.Dispose();
            _maxHealthChangedDisposable.Dispose();

            _isDeadChangedDisposable.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _currentHealth = entity.CurrentHealth;
            _maxHealth = entity.MaxHealth;
            _isDead = entity.IsDead;

            _curentHealthChangedDisposable = _currentHealth.Subscribe(OnHealthChanged);
            _maxHealthChangedDisposable = _maxHealth.Subscribe(OnHealthChanged);
            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);

            OnHealthChanged(0, 0);
        }

        private void OnHealthChanged(float arg1, float arg2)
        {
            float barValue = _currentHealth.Value / _maxHealth.Value;

            _healthBar.UpdateSliderValue(barValue);
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead == true)
                _healthBar.Hide();
        }
    }
}
