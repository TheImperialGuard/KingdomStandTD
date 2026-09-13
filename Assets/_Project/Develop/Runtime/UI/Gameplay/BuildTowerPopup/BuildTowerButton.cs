using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    [RequireComponent(typeof(Button))]
    public class BuildTowerButton : MonoBehaviour, IView
    {
        public event Action<TowerTypes> Clicked;

        [SerializeField] private TowerTypes _towerType;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Color _disabledPriceColor;
        [SerializeField] private Color _enabledPriceColor;

        public TowerTypes TowerType => _towerType;

        private void OnValidate()
        {
            _button = _button != null ? _button : GetComponent<Button>();
        }
        public void OnInit()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void SwitchInteractableOn(bool value)
        {
            _button.interactable = value;
            _price.color = value ? _enabledPriceColor : _disabledPriceColor;
        }

        public void SetPrice(string value) => _price.text = value;

        public void OnDispose()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(_towerType);
        }
    }
}
