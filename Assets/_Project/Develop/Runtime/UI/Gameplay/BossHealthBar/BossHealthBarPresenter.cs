using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BossHealthBar
{
    // Полоска одного босса: боссов на уровне больше одного не бывает, поэтому второй, появившийся при живом первом
    // (только на тестовых уровнях), не показывается. Снятый без гибели босс (дошёл до замка) уходит так же, как погибший
    public class BossHealthBarPresenter : IPresenter
    {
        private readonly BossHealthBarView _view;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private readonly List<IDisposable> _disposables = new();

        private Entity _boss;
        private IReadOnlyVariable<float> _currentHealth;
        private IReadOnlyVariable<float> _maxHealth;

        public BossHealthBarPresenter(BossHealthBarView view, EntitiesLifeContext entitiesLifeContext)
        {
            _view = view;
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void Initialize()
        {
            _entitiesLifeContext.Added += OnEntityAdded;
            _entitiesLifeContext.Released += OnEntityReleased;

            foreach (Entity entity in _entitiesLifeContext.Entities)
                OnEntityAdded(entity);
        }

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
            _entitiesLifeContext.Released -= OnEntityReleased;

            ForgetBoss();
        }

        private void OnEntityAdded(Entity entity)
        {
            if (_boss != null || entity.HasComponent<IsBoss>() == false)
                return;

            _boss = entity;
            _currentHealth = entity.CurrentHealth;
            _maxHealth = entity.MaxHealth;

            _disposables.Add(_currentHealth.Subscribe(OnHealthChanged));
            _disposables.Add(_maxHealth.Subscribe(OnHealthChanged));
            _disposables.Add(entity.IsDead.Subscribe(OnIsDeadChanged));

            _view.Show(entity.EnemyName.Value, GetHealth());
        }

        private void OnEntityReleased(Entity entity)
        {
            if (entity != _boss)
                return;

            bool isDead = _boss.IsDead.Value;

            ForgetBoss();

            if (isDead == false)
                _view.Leave();
        }

        private void OnHealthChanged(float oldValue, float newValue) => _view.SetHealth(GetHealth());

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead == true)
                _view.Leave();
        }

        private float GetHealth() => Mathf.Clamp01(_currentHealth.Value / _maxHealth.Value);

        private void ForgetBoss()
        {
            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
            _boss = null;
        }
    }
}
