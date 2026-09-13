using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using TMPro;
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

        [SerializeField] private TMP_Text _firstAbilityPrice;
        [SerializeField] private TMP_Text _secondAbilityPrice;
        [SerializeField] private GameObject _firstAbilityPriceParent;
        [SerializeField] private GameObject _secondAbilityPriceParent;
        [SerializeField] private Color _disabledPriceColor;
        [SerializeField] private Color _enabledPriceColor;

        [SerializeField] private Image _firstAbilityImage;
        [SerializeField] private Image _secondAbilityImage;

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

        public void SetFirstAbilityPrice(string value) => _firstAbilityPrice.text = value;

        public void SetSecondAbilityPrice(string value) => _secondAbilityPrice.text = value;

        public void SetFirstAbilitySprite(Sprite sprite) => _firstAbilityImage.sprite = sprite;

        public void SetSecondAbilitySprite(Sprite sprite) => _secondAbilityImage.sprite = sprite;

        public void HideFirstAbilityPrice()
        {
            _firstAbilityPriceParent.SetActive(false);
        }

        public void HideSecondAbilityPrice()
        {
            _secondAbilityPriceParent.SetActive(false);
        }

        public void SwitchFirstAbilityInteractable(bool value)
        {
            _firstAbilityButton.interactable = value;
            _firstAbilityPrice.color = value ? _enabledPriceColor : _disabledPriceColor;
        }

        public void SwitchSecondAbilityInteractable(bool value)
        {
            _secondAbilityButton.interactable = value;
            _secondAbilityPrice.color = value ? _enabledPriceColor : _disabledPriceColor;
        }

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

        private void OnFirstAbilityButtonClicked() => FirstAbilityButtonClicked?.Invoke();
        private void OnSecondAbilityButtonClicked() => SecondAbilityButtonClicked?.Invoke();
    }
}
