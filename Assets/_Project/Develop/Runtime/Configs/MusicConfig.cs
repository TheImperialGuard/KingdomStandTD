using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Configs/MusicConfig", fileName = "MusicConfig")]
    public class MusicConfig : ScriptableObject
    {
        [field: SerializeField] public AudioClip MainMenuClip { get; private set; }
        [field: SerializeField] public AudioClip GameplayPrepareClip { get; private set; }
        [field: SerializeField] public AudioClip GameplayBattleClip { get; private set; }
        [field: SerializeField] public AudioClip GameplayBossfightClip { get; private set; }
        [field: SerializeField] public AudioClip GameplayWinClip { get; private set; }
        [field: SerializeField] public AudioClip GameplayDefeatClip { get; private set; }
    }
}
