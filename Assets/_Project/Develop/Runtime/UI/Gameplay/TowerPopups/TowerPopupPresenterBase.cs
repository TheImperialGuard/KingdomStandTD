using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups
{
    // Окно башни или места под неё. Кнопка срабатывает со второго нажатия: первое заряжает её и открывает подсказку,
    // нажатие на другую кнопку переносит заряд, недоступная кнопка нажатий не принимает. Щелчок мимо закрывает окно,
    // щелчок по своей башне ничего не делает (Р-38).
    public abstract class TowerPopupPresenterBase<TButton> : PopupPresenterBase where TButton : struct, Enum
    {
        private const string SellTitle = "Продать башню?";
        private const string SellDescription = "Вы получите золото за продажу: ";
        private const string GainSign = "+";

        private readonly TowerPopupViewBase _view;
        private readonly WalletService _walletService;

        private TButton? _armedButton;

        private IDisposable _goldDisposable;
        private IDisposable _cameraMovedDisposable;

        protected TowerPopupPresenterBase(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            TowerPopupViewBase view,
            Entity source,
            TowersPurchaseService towersPurchaseService,
            TowersPlaceholdersService towersPlaceholdersService,
            WalletService walletService)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            Source = source;
            TowersPurchaseService = towersPurchaseService;
            TowersPlaceholdersService = towersPlaceholdersService;
            _walletService = walletService;
        }

        protected override PopupViewBase PopupView => _view;

        protected Entity Source { get; }

        protected TowersPurchaseService TowersPurchaseService { get; }

        protected TowersPlaceholdersService TowersPlaceholdersService { get; }

        public override void Initialize()
        {
            base.Initialize();

            _view.UpdateWorldPosition(Source.Transform.position);
            _view.HideTip();

            _goldDisposable = _walletService.GetCurrency(CurrencyTypes.Gold).Subscribe(OnGoldChanged);

            BoundedOrthoCamera camera = Camera.main.GetComponent<BoundedOrthoCamera>();
            _cameraMovedDisposable = camera.CameraMoved.Subscribe(OnCameraMoved);

            RefreshButtons();
        }

        public override void Dispose()
        {
            base.Dispose();

            _goldDisposable.Dispose();
            _cameraMovedDisposable.Dispose();
        }

        protected abstract void RefreshButtons();

        protected abstract bool IsAvailable(TButton button);

        protected abstract TowerTipContent GetTip(TButton button);

        protected abstract void Activate(TButton button);

        protected virtual bool IsMaxLevel(TButton button) => false;

        protected virtual void OnArmed(TButton button) { }

        protected virtual void OnDisarmed(TButton button) { }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            if (newHit.IsOn(Source))
                return;

            Disarm();
            OnCloseRequest();
        }

        protected void OnButtonClicked(TButton button)
        {
            if (IsAvailable(button) == false)
                return;

            if (IsArmed(button))
            {
                Activate(button);
                return;
            }

            Arm(button);
        }

        protected void Disarm()
        {
            if (_armedButton.HasValue == false)
                return;

            TButton button = _armedButton.Value;

            _armedButton = null;
            _view.HideTip();

            OnDisarmed(button);
            RefreshButtons();
        }

        protected MedallionStates GetStateFor(TButton button)
        {
            if (IsMaxLevel(button))
                return MedallionStates.Max;

            if (IsAvailable(button) == false)
                return MedallionStates.Unavailable;

            return IsArmed(button) ? MedallionStates.Armed : MedallionStates.Idle;
        }

        protected TowerTipContent GetSellTip()
        {
            int gold = TowersPurchaseService.GetGoldAmountForSell(Source);

            return new TowerTipContent(SellTitle, SellDescription + gold, bigNumber: GainSign + gold);
        }

        protected void SellSource()
        {
            TowersPurchaseService.Sell(Source);
            TowersPlaceholdersService.CreatePlaceholder(Source.Transform.position);

            Source.SelfReleaseRequested.Value = true;
            OnCloseRequest(false);
        }

        private bool IsArmed(TButton button)
            => _armedButton.HasValue && EqualityComparer<TButton>.Default.Equals(_armedButton.Value, button);

        private void Arm(TButton button)
        {
            Disarm();

            _armedButton = button;

            RefreshButtons();
            _view.ShowTip(GetTip(button));
            OnArmed(button);
        }

        // Золото в открытом окне только прибывает, но заряженная кнопка, ставшая недоступной, заряд всё равно теряет.
        private void OnGoldChanged(int oldValue, int newValue)
        {
            if (_armedButton.HasValue && IsAvailable(_armedButton.Value) == false)
            {
                Disarm();
                return;
            }

            RefreshButtons();
        }

        private void OnCameraMoved() => _view.UpdateWorldPosition(Source.Transform.position);
    }
}
