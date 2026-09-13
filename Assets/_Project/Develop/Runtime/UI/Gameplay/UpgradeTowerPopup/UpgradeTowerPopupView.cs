using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using TMPro;
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
        [SerializeField] private TMP_Text _upgradePrice;
        [SerializeField] private Color _disabledPriceColor;
        [SerializeField] private Color _enabledPriceColor;

        [SerializeField] private TitleWithTextView _leftInfoContainer;
        [SerializeField] private TitleWithTextView _rightInfoContainer;

        private Camera _camera;

        public void UpdateWorldPosition(Vector3 worldPosition)
        {
            Vector3 position = _camera.WorldToScreenPoint(worldPosition);

            transform.position = position;
        }

        public void SetupInfoContainer(string title, string desc)
        {
            _leftInfoContainer.SetTitle(title);
            _rightInfoContainer.SetTitle(title);

            _leftInfoContainer.SetText(desc);
            _rightInfoContainer.SetText(desc);
        }

        public void ShowInfoContainer(RelativeUIPositions position)
        {
            GetContainerBy(position).gameObject.SetActive(true);
        }

        public void HideInfoContainer()
        {
            _leftInfoContainer.gameObject.SetActive(false);
            _rightInfoContainer.gameObject.SetActive(false);
        }

        public void SwitchUpgradeInteractable(bool value)
        {
            _upgradeButton.interactable = value;
            _upgradePrice.color = value ? _enabledPriceColor : _disabledPriceColor;
        }

        public void SetUpgradePrice(string value) => _upgradePrice.text = value;

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

        private TitleWithTextView GetContainerBy(RelativeUIPositions position)
        {
            return position switch
            {
                RelativeUIPositions.Left => _leftInfoContainer,
                RelativeUIPositions.Right => _rightInfoContainer,
                _ => throw new NotImplementedException(),
            };
        }

        private void OnSellButtonClicked() => SellButtonClicked?.Invoke();

        private void OnUpgradeButtonClicked() => UpgradeButtonClicked?.Invoke();
    }
}
