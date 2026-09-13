using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    public class BuildTowerPopupView : PopupViewBase, IWorldPositionView
    {
        public event Action<TowerTypes> BuildTowerButtonClicked;

        [SerializeField] private List<BuildTowerButton> _towersButtons;

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

        public void SwitchInteractableFor(TowerTypes type, bool value)
        {
            BuildTowerButton towerButton = _towersButtons.Where((button) => button.TowerType == type).First();

            towerButton.SwitchInteractableOn(value);
        }

        public void SetPriceFor(TowerTypes type, string value)
        {
            BuildTowerButton towerButton = _towersButtons.Where((button) => button.TowerType == type).First();

            towerButton.SetPrice(value);
        }

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            foreach (BuildTowerButton button in _towersButtons)
            {
                button.OnInit();
                button.Clicked += OnBuildTowerButtonClicked;
            }
        }

        private void OnDisable()
        {
            foreach (BuildTowerButton button in _towersButtons)
            {
                button.OnDispose();
                button.Clicked -= OnBuildTowerButtonClicked;
            }
        }

        private void OnBuildTowerButtonClicked(TowerTypes type)
        {
            BuildTowerButtonClicked?.Invoke(type);
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
    }
}
