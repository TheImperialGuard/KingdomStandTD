using Assets._Project.Develop.Runtime.UI.Core.Popups;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.PausePopup
{
    public class PausePopupView : PopupViewBase
    {
        public event Action ExitClicked;
        public event Action RestartClicked;

        public event Action<float> MusicVolumeChanged;
        public event Action<float> SoundsVolumeChanged;

        public event Action<bool> MusicToggleChanged;
        public event Action<bool> SoundsToggleChanged;

        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundsSlider;

        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundsToggle;

        public void SetupMusicHandlers(bool isOn, float volume)
        {
            _musicSlider.minValue = -80f;
            _musicSlider.maxValue = 0f;

            _musicSlider.value = volume;
            _musicToggle.isOn = isOn;
        }

        public void SetupSoundsHandlers(bool isOn, float volume)
        {
            _soundsSlider.minValue = -80f;
            _soundsSlider.maxValue = 0f;

            _soundsSlider.value = volume;
            _soundsToggle.isOn = isOn;
        }

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _exitButton.onClick.AddListener(OnExitButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _musicSlider.onValueChanged.AddListener(OnMusicValueChanged);
            _soundsSlider.onValueChanged.AddListener(OnSoundsValueChanged);
            _musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            _soundsToggle.onValueChanged.AddListener(OnSoundsToggleChanged);
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

        private void OnSoundsValueChanged(float value) => SoundsVolumeChanged?.Invoke(value);

        private void OnMusicValueChanged(float value) => MusicVolumeChanged?.Invoke(value);

        private void OnSoundsToggleChanged(bool value) => SoundsToggleChanged?.Invoke(value);

        private void OnMusicToggleChanged(bool value) => MusicToggleChanged?.Invoke(value);
    }
}
