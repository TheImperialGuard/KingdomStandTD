using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    public class BuildTowerPopupPresenter : PopupPresenterBase
    {
        private readonly BuildTowerPopupView _view;

        private readonly Entity _towerPlaceholder;
        private readonly RayShooterService _rayShooterService;

        private IDisposable _rayShooterServiceDisposable;

        public BuildTowerPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            BuildTowerPopupView view,
            Entity towerPlaceholder,
            RayShooterService rayShooterService)
            : base(coroutinesPerformer)
        {
            _view = view;
            _towerPlaceholder = towerPlaceholder;
            _rayShooterService = rayShooterService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.UpdatePosition(_towerPlaceholder.Transform.position);

            _rayShooterServiceDisposable = _rayShooterService.LastHitInfo.Subscribe(OnClickedOutside);

            _view.BuildTowerButtonClicked += OnBuildTowerButtonClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _rayShooterServiceDisposable.Dispose();

            _view.BuildTowerButtonClicked -= OnBuildTowerButtonClicked;
        }

        private void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            OnCloseRequest();
        }

        private void OnBuildTowerButtonClicked(TowerTypes type)
        {
            Debug.Log($"Вы кликнули на иконку башни {type}");
        }
    }
}
