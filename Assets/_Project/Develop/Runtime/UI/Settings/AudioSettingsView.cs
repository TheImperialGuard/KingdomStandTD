using System;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Settings
{
    public class AudioSettingsView : MonoBehaviour, IView
    {
        public event Action<float> MusicVolumeChanged;
        public event Action<float> SoundsVolumeChanged;

        public event Action<bool> MusicToggleChanged;
        public event Action<bool> SoundsToggleChanged;

        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundsSlider;

        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundsToggle;

        public void SetMusic(bool isOn, float volume)
        {
            _musicSlider.SetValueWithoutNotify(volume);
            _musicToggle.SetIsOnWithoutNotify(isOn);
        }

        public void SetSounds(bool isOn, float volume)
        {
            _soundsSlider.SetValueWithoutNotify(volume);
            _soundsToggle.SetIsOnWithoutNotify(isOn);
        }

        private void OnEnable()
        {
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _soundsSlider.onValueChanged.AddListener(OnSoundsVolumeChanged);
            _musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            _soundsToggle.onValueChanged.AddListener(OnSoundsToggleChanged);
        }

        private void OnDisable()
        {
            _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            _soundsSlider.onValueChanged.RemoveListener(OnSoundsVolumeChanged);
            _musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
            _soundsToggle.onValueChanged.RemoveListener(OnSoundsToggleChanged);
        }

        private void OnMusicVolumeChanged(float volume) => MusicVolumeChanged?.Invoke(volume);

        private void OnSoundsVolumeChanged(float volume) => SoundsVolumeChanged?.Invoke(volume);

        private void OnMusicToggleChanged(bool isOn) => MusicToggleChanged?.Invoke(isOn);

        private void OnSoundsToggleChanged(bool isOn) => SoundsToggleChanged?.Invoke(isOn);
    }
}
