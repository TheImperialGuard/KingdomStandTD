using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action OpenLevelsMenuButtonClicked;

        [SerializeField] private Button _openLevelsMenuButton;

        private void OnEnable()
        {
            _openLevelsMenuButton.onClick.AddListener(OnOpenLevelsMenuButtonClicked);
        }

        private void OnDisable()
        {
            _openLevelsMenuButton.onClick.RemoveListener(OnOpenLevelsMenuButtonClicked);
        }

        private void OnOpenLevelsMenuButtonClicked() => OpenLevelsMenuButtonClicked?.Invoke();
    }
}
