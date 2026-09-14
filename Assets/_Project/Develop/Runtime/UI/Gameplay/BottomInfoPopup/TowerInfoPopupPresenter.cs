using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup
{
    public class TowerInfoPopupPresenter : PopupPresenterBase
    {
        private readonly BottomInfoPopupView _view;
        private readonly Entity _sourceTower;

        private readonly EntitiesLifeContext _entitiesLifeContext;

        private IDisposable _damageChangedDisposable;

        public TowerInfoPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            BottomInfoPopupView view,
            Entity sourceTower,
            EntitiesLifeContext entitiesLifeContext)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _sourceTower = sourceTower;
            _entitiesLifeContext = entitiesLifeContext;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _damageChangedDisposable = _sourceTower.InstantAttackDamage.Subscribe(OnDamageChanged);
            _entitiesLifeContext.Released += OnSomeEntityReleased;

            SetupInfo();
        }

        private void SetupInfo()
        {
            _view.SetTitle(_sourceTower.TowerName.Value);
            _view.SetTowerDesc(_sourceTower.InstantAttackDamage.Value.ToString(), _sourceTower.InstantAttackDamageType.Value);
        }

        private void OnDamageChanged(float arg1, float damage) => SetupInfo();

        public override void Dispose()
        {
            base.Dispose();

            _damageChangedDisposable?.Dispose();
            _entitiesLifeContext.Released -= OnSomeEntityReleased;
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            OnCloseRequest();
        }

        private void OnSomeEntityReleased(Entity entity)
        {
            if (_sourceTower == entity)
                OnCloseRequest();
        }
    }
}
