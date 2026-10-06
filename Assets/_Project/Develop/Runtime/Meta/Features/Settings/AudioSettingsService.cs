using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;

namespace Assets._Project.Develop.Runtime.Meta.Features.Settings
{
    public class AudioSettingsService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private readonly AudioHandler _audioHandler;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private AudioSettingsData _savedSettings = new();

        public AudioSettingsService(
            AudioHandler audioHandler,
            PlayerDataProvider playerDataProvider,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _audioHandler = audioHandler;
            _playerDataProvider = playerDataProvider;
            _coroutinesPerformer = coroutinesPerformer;

            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public void SaveIfChanged()
        {
            if (HasUnsavedChanges() == false)
                return;

            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());
        }

        public void ReadFrom(PlayerData data)
        {
            AudioSettingsData settings = data.AudioSettings ?? new AudioSettingsData();

            _audioHandler.SetMusicVolume(settings.MusicVolume);
            _audioHandler.SetSoundsVolume(settings.SoundsVolume);
            _audioHandler.SetMusicOn(settings.IsMusicOn);
            _audioHandler.SetSoundsOn(settings.IsSoundsOn);

            _savedSettings = CaptureCurrentSettings();
        }

        public void WriteTo(PlayerData data)
        {
            _savedSettings = CaptureCurrentSettings();

            data.AudioSettings = _savedSettings;
        }

        private bool HasUnsavedChanges()
        {
            return _audioHandler.MusicVolume != _savedSettings.MusicVolume
                || _audioHandler.SoundsVolume != _savedSettings.SoundsVolume
                || _audioHandler.IsMusicOn != _savedSettings.IsMusicOn
                || _audioHandler.IsSoundsOn != _savedSettings.IsSoundsOn;
        }

        private AudioSettingsData CaptureCurrentSettings()
        {
            return new AudioSettingsData
            {
                MusicVolume = _audioHandler.MusicVolume,
                SoundsVolume = _audioHandler.SoundsVolume,
                IsMusicOn = _audioHandler.IsMusicOn,
                IsSoundsOn = _audioHandler.IsSoundsOn
            };
        }
    }
}
