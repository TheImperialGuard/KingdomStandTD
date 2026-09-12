using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
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

        private Camera _camera;

        public void UpdateWorldPosition(Vector3 worldPosition)
        {
            Vector3 position = _camera.WorldToScreenPoint(worldPosition);

            transform.position = position;
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
    }
}
