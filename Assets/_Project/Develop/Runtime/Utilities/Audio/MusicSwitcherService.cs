using Assets._Project.Develop.Runtime.Configs;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Audio
{
    public class MusicSwitcherService
    {
        private readonly MusicPlayer _musicPlayer;
        private readonly MusicConfig _musicConfig;

        public MusicSwitcherService(MusicPlayer musicPlayer, MusicConfig musicConfig)
        {
            _musicPlayer = musicPlayer;
            _musicConfig = musicConfig;
        }

        public void SwitchFor(MusicContexts context, bool IsLoop = true)
        {
            AudioClip clip = _musicConfig.GetClipFor(context);
            Play(clip);

            _musicPlayer.SetLoop(IsLoop);
        }

        private void Play(AudioClip clip)
        {
            _musicPlayer.Stop();
            _musicPlayer.SetMusic(clip);
            _musicPlayer.Play();
        }
    }
}