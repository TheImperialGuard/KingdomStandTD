using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup
{
    public class EnemyInfoPopupPresenter : PopupPresenterBase
    {
        private readonly BottomInfoPopupView _view;
        private readonly Entity _sourceEnemy;

        private IDisposable _maxHealthChangedDisposable;

        public EnemyInfoPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            BottomInfoPopupView view,
            Entity sourceTower)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _sourceEnemy = sourceTower;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _maxHealthChangedDisposable = _sourceEnemy.MaxHealth.Subscribe(OnMaxHealthChanged);

            SetupInfo();
        }

        private void SetupInfo()
        {
            _view.SetTitle(_sourceEnemy.EnemyName.Value);
            _view.SetEnemyDesc(_sourceEnemy.MaxHealth.Value.ToString(), _sourceEnemy.DamageResistanceType.Value);
        }

        private void OnMaxHealthChanged(float arg1, float damage) => SetupInfo();

        public override void Dispose()
        {
            base.Dispose();

            _maxHealthChangedDisposable?.Dispose();
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            OnCloseRequest();
        }
    }
}
