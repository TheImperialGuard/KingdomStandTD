using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    public class BuildTowerPopupView : TowerPopupViewBase
    {
        public event Action<TowerTypes> ButtonClicked;

        [SerializeField] private TowerMedallionView _arrowsButton;
        [SerializeField] private TowerMedallionView _magicButton;
        [SerializeField] private TowerMedallionView _cannonButton;

        public void SetButton(TowerTypes type, MedallionStates state, int cost) => GetButton(type).Set(state, cost);

        private void OnEnable()
        {
            _arrowsButton.Clicked += OnArrowsButtonClicked;
            _magicButton.Clicked += OnMagicButtonClicked;
            _cannonButton.Clicked += OnCannonButtonClicked;
        }

        private void OnDisable()
        {
            _arrowsButton.Clicked -= OnArrowsButtonClicked;
            _magicButton.Clicked -= OnMagicButtonClicked;
            _cannonButton.Clicked -= OnCannonButtonClicked;
        }

        private TowerMedallionView GetButton(TowerTypes type)
        {
            return type switch
            {
                TowerTypes.Arrows => _arrowsButton,
                TowerTypes.Magic => _magicButton,
                TowerTypes.Cannon => _cannonButton,
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }

        private void OnArrowsButtonClicked() => ButtonClicked?.Invoke(TowerTypes.Arrows);

        private void OnMagicButtonClicked() => ButtonClicked?.Invoke(TowerTypes.Magic);

        private void OnCannonButtonClicked() => ButtonClicked?.Invoke(TowerTypes.Cannon);
    }
}
