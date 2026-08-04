using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup
{
    public class UpgradeTowerPopupView : PopupViewBase, IWorldPositionView
    {
        public event Action UpgradeButtonClicked;
        public event Action SellButtonClicked;

        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _sellButton;

        private Camera _camera;

        public void UpdateWorldPosition(Vector3 worldPosition)
        {
            Vector3 position = _camera.WorldToScreenPoint(worldPosition);

            transform.position = position;
        }

        public void SwitchUpgradeInteractable(bool value) => _upgradeButton.interactable = value;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            _sellButton.onClick.AddListener(OnSellButtonClicked);
        }

        private void OnDisable()
        {
            _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
            _sellButton.onClick.RemoveListener(OnSellButtonClicked);
        }

        private void OnSellButtonClicked() => SellButtonClicked?.Invoke();

        private void OnUpgradeButtonClicked() => UpgradeButtonClicked?.Invoke();
    }
}
