using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    public class BuildTowerPopupView : PopupViewBase
    {
        public event Action<TowerTypes> BuildTowerButtonClicked;

        [SerializeField] private List<BuildTowerButton> _towersButtons;

        private Camera _camera;

        public void UpdatePosition(Vector3 worldPosition)
        {
            Vector3 position = _camera.WorldToScreenPoint(worldPosition);

            transform.position = position;
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
