using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup
{
    public class EnemyInfoPopupPresenter : PopupPresenterBase
    {
        private const string HealthSeparator = " / ";

        private readonly BottomInfoPopupView _view;
        private readonly Entity _sourceEnemy;

        private readonly EntitiesLifeContext _entitiesLifeContext;

        private IDisposable _currentHealthChangedDisposable;
        private IDisposable _maxHealthChangedDisposable;

        public EnemyInfoPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            BottomInfoPopupView view,
            Entity sourceEnemy,
            EntitiesLifeContext entitiesLifeContext)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _sourceEnemy = sourceEnemy;
            _entitiesLifeContext = entitiesLifeContext;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _currentHealthChangedDisposable = _sourceEnemy.CurrentHealth.Subscribe(OnHealthChanged);
            _maxHealthChangedDisposable = _sourceEnemy.MaxHealth.Subscribe(OnHealthChanged);
            _entitiesLifeContext.Released += OnSomeEntityReleased;

            SetupInfo();
        }

        private void SetupInfo()
        {
            _view.SetEnemyInfo(
                _sourceEnemy.EnemyName.Value,
                _sourceEnemy.DamageResistanceType.Value,
                _sourceEnemy.HasComponent<IsBoss>(),
                _sourceEnemy.HasComponent<IsFlying>());

            UpdateHealth();
        }

        // Здоровье округляется вверх: живой враг с долей единицы здоровья не показывает ноль.
        private void UpdateHealth()
        {
            float currentHealth = _sourceEnemy.CurrentHealth.Value;
            float maxHealth = _sourceEnemy.MaxHealth.Value;
            int shownCurrentHealth = Mathf.CeilToInt(currentHealth);
            int shownMaxHealth = Mathf.CeilToInt(maxHealth);

            _view.SetEnemyHealth(shownCurrentHealth + HealthSeparator + shownMaxHealth, currentHealth / maxHealth);
        }

        private void OnHealthChanged(float oldHealth, float newHealth) => UpdateHealth();

        public override void Dispose()
        {
            base.Dispose();

            _currentHealthChangedDisposable?.Dispose();
            _maxHealthChangedDisposable?.Dispose();
            _entitiesLifeContext.Released -= OnSomeEntityReleased;
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            OnCloseRequest();
        }

        private void OnSomeEntityReleased(Entity entity)
        {
            if (_sourceEnemy == entity)
                OnCloseRequest();
        }
    }
}
