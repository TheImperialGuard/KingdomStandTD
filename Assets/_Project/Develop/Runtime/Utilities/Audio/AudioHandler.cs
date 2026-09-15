using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Utilities.Audio
{
    public class AudioHandler
    {
        private const float OffVolume = -80f;
        private const float MaxVolume = 0f;

        public const string MusicKey = "MusicVolume";
        public const string SoundsKey = "SoundsVolume";

        private readonly AudioMixer _audioMixer;

        private float _musicVolume = 0f;
        private float _soundsVolume = 0f;

        private bool _isMusicOn = true;
        private bool _isSoundsOn = true;

        public AudioHandler(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;

            SetMusicVolume(MaxVolume);
            SetSoundsVolume(MaxVolume);
        }

        public bool IsMusicOn => _isMusicOn;
        public bool IsSoundsOn => _isSoundsOn;
        public float MusicVolume => _musicVolume;
        public float SoundsVolume => _soundsVolume;

        public void SetMusicVolume(float volumeLevel)
        {
            volumeLevel = ValidateVolume(volumeLevel);

            _musicVolume = volumeLevel;

            _audioMixer.SetFloat(MusicKey, volumeLevel);
        }

        public void SetSoundsVolume(float volumeLevel)
        {
            volumeLevel = ValidateVolume(volumeLevel);

            _soundsVolume = volumeLevel;

            _audioMixer.SetFloat(SoundsKey, volumeLevel);
        }

        public void OnMusic()
        {
            _audioMixer.SetFloat(MusicKey, _musicVolume);
            _isMusicOn = true;
        }

        public void OnSounds()
        {
            _audioMixer.SetFloat(SoundsKey, _soundsVolume);
            _isSoundsOn = true;
        }

        public void OffMusic()
        {
            _audioMixer.SetFloat(MusicKey, OffVolume);
            _isMusicOn = false;
        }

        public void OffSounds()
        {
            _audioMixer.SetFloat(SoundsKey, OffVolume);
            _isSoundsOn = false;
        }

        private float ValidateVolume(float volumeLevel)
        {
            float finalVolume = volumeLevel;

            if (finalVolume < OffVolume)
                finalVolume = OffVolume;

            if (finalVolume > MaxVolume)
                finalVolume = MaxVolume;

            return finalVolume;
        }
    }
}