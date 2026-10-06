using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action OpenLevelsMenuButtonClicked;
        public event Action OpenSettingsButtonClicked;

        [SerializeField] private Button _openLevelsMenuButton;
        [SerializeField] private Button _openSettingsButton;

        private void OnEnable()
        {
            _openLevelsMenuButton.onClick.AddListener(OnOpenLevelsMenuButtonClicked);
            _openSettingsButton.onClick.AddListener(OnOpenSettingsButtonClicked);
        }

        private void OnDisable()
        {
            _openLevelsMenuButton.onClick.RemoveListener(OnOpenLevelsMenuButtonClicked);
            _openSettingsButton.onClick.RemoveListener(OnOpenSettingsButtonClicked);
        }

        private void OnOpenLevelsMenuButtonClicked() => OpenLevelsMenuButtonClicked?.Invoke();

        private void OnOpenSettingsButtonClicked() => OpenSettingsButtonClicked?.Invoke();
    }
}
