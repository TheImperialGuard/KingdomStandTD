using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.UpgradeTowerPopup
{
    public class UpgradeTowerPopupView : TowerPopupViewBase
    {
        public event Action<UpgradeTowerPopupButtons> ButtonClicked;

        [SerializeField] private TowerMedallionView _upgradeButton;
        [SerializeField] private MedallionButtonView _sellButton;

        public void SetUpgradeButton(MedallionStates state, int cost) => _upgradeButton.Set(state, cost);

        public void SetSellButton(MedallionStates state) => _sellButton.SetState(state);

        private void OnEnable()
        {
            _upgradeButton.Clicked += OnUpgradeButtonClicked;
            _sellButton.Clicked += OnSellButtonClicked;
        }

        private void OnDisable()
        {
            _upgradeButton.Clicked -= OnUpgradeButtonClicked;
            _sellButton.Clicked -= OnSellButtonClicked;
        }

        private void OnUpgradeButtonClicked() => ButtonClicked?.Invoke(UpgradeTowerPopupButtons.Upgrade);

        private void OnSellButtonClicked() => ButtonClicked?.Invoke(UpgradeTowerPopupButtons.Sell);
    }
}
