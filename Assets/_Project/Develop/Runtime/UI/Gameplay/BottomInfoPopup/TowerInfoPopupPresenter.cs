using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Text;
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
        private IDisposable _frozenDisposable;

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
            _frozenDisposable = _sourceTower.IsFrozen.Subscribe(OnFrozenChanged);
            _entitiesLifeContext.Released += OnSomeEntityReleased;

            SetupInfo();
        }

        private void SetupInfo()
        {
            _view.SetTowerInfo(
                NonBreakingSpace.BeforeTrailingNumber(_sourceTower.TowerName.Value),
                _sourceTower.InstantAttackDamageType.Value,
                DecimalText.Format(_sourceTower.AttacksPerSecond.Value),
                DecimalText.Format(_sourceTower.InstantShootRange.Value));

            UpdateDamage();
        }

        private void UpdateDamage() => _view.SetTowerDamage(DecimalText.Format(_sourceTower.InstantAttackDamage.Value));

        private void OnDamageChanged(float arg1, float damage) => UpdateDamage();

        public override void Dispose()
        {
            base.Dispose();

            _damageChangedDisposable?.Dispose();
            _frozenDisposable?.Dispose();
            _entitiesLifeContext.Released -= OnSomeEntityReleased;
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            // Щелчок по своей башне ничего не делает, как и в окне башни (Р-38).
            if (newHit.IsOn(_sourceTower))
                return;

            OnCloseRequest();
        }

        private void OnSomeEntityReleased(Entity entity)
        {
            if (_sourceTower == entity)
                OnCloseRequest();
        }

        // Закрывается вместе с окном замороженной башни.
        private void OnFrozenChanged(bool oldValue, bool isFrozen)
        {
            if (isFrozen)
                OnCloseRequest();
        }
    }
}
