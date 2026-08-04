using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Core.Popups
{
    public abstract class PopupPresenterBase : IPresenter
    {
        public event Action<PopupPresenterBase, bool> CloseRequest;

        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly RayShooterService _rayShooterService;

        private IDisposable _rayShooterServiceDisposable;

        private Coroutine _process;

        protected PopupPresenterBase(
            ICoroutinesPerformer coroutinesPerformer, 
            RayShooterService rayShooterService)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _rayShooterService = rayShooterService;
        }

        protected abstract PopupViewBase PopupView { get; }

        public virtual void Initialize()
        {
            _rayShooterServiceDisposable = _rayShooterService.LastHitInfo.Subscribe(OnClickedOutside);
        }

        public virtual void Dispose()
        {
            KillProcess();

            PopupView.CloseRequest -= OnCloseRequest;

            _rayShooterServiceDisposable.Dispose();
        }

        public void Show()
        {
            KillProcess();

            _process = _coroutinesPerformer.StartPerform(ProcessShow());
        }

        public void Hide(Action callback = null)
        {
            KillProcess();

            _process = _coroutinesPerformer.StartPerform(ProcessHide(callback));
        }

        protected virtual void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit) { }

        protected virtual void OnPreShow()
        {
            PopupView.CloseRequest += OnCloseRequest;
        }

        protected virtual void OnPostShow() { }

        protected virtual void OnPreHide()
        {
            PopupView.CloseRequest -= OnCloseRequest;
        }

        protected virtual void OnPostHide() { }

        protected void OnCloseRequest(bool withCallBack = true) => CloseRequest?.Invoke(this, withCallBack);

        private IEnumerator ProcessShow()
        {
            OnPreShow();

            yield return PopupView.Show().WaitForCompletion();

            OnPostShow();
        }

        private IEnumerator ProcessHide(Action callback)
        {
            OnPreHide();

            yield return PopupView.Hide().WaitForCompletion();

            OnPostHide();

            callback?.Invoke();
        }

        private void KillProcess()
        {
            if (_process != null)
                _coroutinesPerformer.StopPerform(_process);
        }
    }
}
