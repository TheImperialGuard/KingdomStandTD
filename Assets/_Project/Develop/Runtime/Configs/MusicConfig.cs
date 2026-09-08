using Assets._Project.Develop.Runtime.Utilities.Audio;
using UnityEngine;
using System;

namespace Assets._Project.Develop.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Configs/MusicConfig", fileName = "MusicConfig")]
    public class MusicConfig : ScriptableObject
    {
        [SerializeField] private AudioClip _mainMenuClip;
        [SerializeField] private AudioClip _gameplayPrepareClip;
        [SerializeField] private AudioClip _gameplayBattleClip;
        [SerializeField] private AudioClip _gameplayBossfightClip;
        [SerializeField] private AudioClip _gameplayWinClip;
        [SerializeField] private AudioClip _gameplayDefeatClip;

        public AudioClip GetClipFor(MusicContexts context)
        {
            AudioClip clip = context switch
            {
                MusicContexts.MainMenu => _mainMenuClip,
                MusicContexts.GameplayPrepare => _gameplayPrepareClip,
                MusicContexts.GameplayBattle => _gameplayBattleClip,
                MusicContexts.GameplayBossfight => _gameplayBossfightClip,
                MusicContexts.GameplayWin => _gameplayWinClip,
                MusicContexts.GameplayDefeat => _gameplayDefeatClip,
                _ => throw new ArgumentException($"{context} not supported in {this}"),
            };

            return clip;
        }
    }
}