using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Utilities.Audio
{
    public class AudioHandler
    {
        private const string MusicVolumeParameter = "MusicVolume";
        private const string SoundsVolumeParameter = "SoundsVolume";

        private const float MaxVolume = 1f;
        private const float SilenceDecibels = -80f;

        private const float HalvingBase = 2f;
        private const float DecibelsPerHalving = 10f;

        private readonly AudioMixer _audioMixer;

        private readonly AudioChannel _music = new(MusicVolumeParameter);
        private readonly AudioChannel _sounds = new(SoundsVolumeParameter);

        public AudioHandler(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;

            ApplyToMixer(_music);
            ApplyToMixer(_sounds);
        }

        public bool IsMusicOn => _music.IsOn;
        public bool IsSoundsOn => _sounds.IsOn;
        public float MusicVolume => _music.Volume;
        public float SoundsVolume => _sounds.Volume;

        public void SetMusicVolume(float volume) => SetVolume(_music, volume);

        public void SetSoundsVolume(float volume) => SetVolume(_sounds, volume);

        public void SetMusicOn(bool isOn) => SetOn(_music, isOn);

        public void SetSoundsOn(bool isOn) => SetOn(_sounds, isOn);

        private void SetVolume(AudioChannel channel, float volume)
        {
            channel.Volume = Mathf.Clamp01(volume);

            ApplyToMixer(channel);
        }

        private void SetOn(AudioChannel channel, bool isOn)
        {
            channel.IsOn = isOn;

            ApplyToMixer(channel);
        }

        private void ApplyToMixer(AudioChannel channel)
        {
            float decibels = channel.IsOn ? ToDecibels(channel.Volume) : SilenceDecibels;

            _audioMixer.SetFloat(channel.MixerParameter, decibels);
        }

        private float ToDecibels(float volume)
        {
            if (volume <= 0f)
                return SilenceDecibels;

            float decibels = Mathf.Log(volume, HalvingBase) * DecibelsPerHalving;

            return Mathf.Max(decibels, SilenceDecibels);
        }

        private class AudioChannel
        {
            public AudioChannel(string mixerParameter)
            {
                MixerParameter = mixerParameter;
            }

            public string MixerParameter { get; }
            public float Volume { get; set; } = MaxVolume;
            public bool IsOn { get; set; } = true;
        }
    }
}
