using System;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.PausePopup
{
    public class PausePopupView : PopupViewBase
    {
        public event Action ExitClicked;
        public event Action RestartClicked;

        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private AudioSettingsView _audioSettingsView;

        public AudioSettingsView AudioSettingsView => _audioSettingsView;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _exitButton.onClick.AddListener(OnExitButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnExitButtonClicked() => ExitClicked?.Invoke();

        private void OnRestartButtonClicked() => RestartClicked?.Invoke();
    }
}
