using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerAbilitiesPopup
{
    public class TowerAbilitiesPopupView : PopupViewBase, IWorldPositionView
    {
        public event Action FirstAbilityButtonClicked;
        public event Action SecondAbilityButtonClicked;
        public event Action SellButtonClicked;

        [SerializeField] private Button _firstAbilityButton;
        [SerializeField] private Button _secondAbilityButton;
        [SerializeField] private Button _sellButton;

        private Camera _camera;

        public void UpdateWorldPosition(Vector3 worldPosition)
        {
            Vector3 position = _camera.WorldToScreenPoint(worldPosition);

            transform.position = position;
        }

        public void SwitchFirstAbilityInteractable(bool value) => _firstAbilityButton.interactable = value;
        public void SwitchSecondAbilityInteractable(bool value) => _secondAbilityButton.interactable = value;

        public void OnAbilityProvided()
        {
            Selectable firstButtonSelectable = _firstAbilityButton.GetComponent<Selectable>();
            Selectable secondButtonSelectable = _secondAbilityButton.GetComponent<Selectable>();

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            firstButtonSelectable.OnDeselect(null);
            secondButtonSelectable.OnDeselect(null);
        }

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _firstAbilityButton.onClick.AddListener(OnFirstAbilityButtonClicked);
            _secondAbilityButton.onClick.AddListener(OnSecondAbilityButtonClicked);
            _sellButton.onClick.AddListener(OnSellButtonClicked);
        }

        private void OnDisable()
        {
            _firstAbilityButton.onClick.RemoveListener(OnFirstAbilityButtonClicked);
            _secondAbilityButton.onClick.RemoveListener(OnSecondAbilityButtonClicked);
            _sellButton.onClick.RemoveListener(OnSellButtonClicked);
        }

        private void OnSellButtonClicked() => SellButtonClicked?.Invoke();

        private void OnFirstAbilityButtonClicked() => FirstAbilityButtonClicked?.Invoke();
        private void OnSecondAbilityButtonClicked() => SecondAbilityButtonClicked?.Invoke();
    }
}
