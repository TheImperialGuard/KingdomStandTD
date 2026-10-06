using Assets._Project.Develop.Runtime.Meta.Features.Settings;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.Audio;

namespace Assets._Project.Develop.Runtime.UI.Settings
{
    public class AudioSettingsPresenter : IPresenter
    {
        private readonly AudioSettingsView _view;
        private readonly AudioHandler _audioHandler;
        private readonly AudioSettingsService _audioSettingsService;

        public AudioSettingsPresenter(
            AudioSettingsView view,
            AudioHandler audioHandler,
            AudioSettingsService audioSettingsService)
        {
            _view = view;
            _audioHandler = audioHandler;
            _audioSettingsService = audioSettingsService;
        }

        public void Initialize()
        {
            _view.SetMusic(_audioHandler.IsMusicOn, _audioHandler.MusicVolume);
            _view.SetSounds(_audioHandler.IsSoundsOn, _audioHandler.SoundsVolume);
        }

        public void Dispose()
        {
            Unsubscribe();

            _audioSettingsService.SaveIfChanged();
        }

        public void Subscribe()
        {
            _view.MusicVolumeChanged += OnMusicVolumeChanged;
            _view.SoundsVolumeChanged += OnSoundsVolumeChanged;

            _view.MusicToggleChanged += OnMusicToggleChanged;
            _view.SoundsToggleChanged += OnSoundsToggleChanged;
        }

        public void Unsubscribe()
        {
            _view.MusicVolumeChanged -= OnMusicVolumeChanged;
            _view.SoundsVolumeChanged -= OnSoundsVolumeChanged;

            _view.MusicToggleChanged -= OnMusicToggleChanged;
            _view.SoundsToggleChanged -= OnSoundsToggleChanged;
        }

        private void OnMusicVolumeChanged(float volume) => _audioHandler.SetMusicVolume(volume);

        private void OnSoundsVolumeChanged(float volume) => _audioHandler.SetSoundsVolume(volume);

        private void OnMusicToggleChanged(bool isOn) => _audioHandler.SetMusicOn(isOn);

        private void OnSoundsToggleChanged(bool isOn) => _audioHandler.SetSoundsOn(isOn);
    }
}
