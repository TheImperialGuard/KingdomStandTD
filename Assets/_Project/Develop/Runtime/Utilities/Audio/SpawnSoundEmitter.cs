using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Audio
{
    public class SpawnSoundEmitter : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private void OnEnable()
        {
            if (SoundRetriggerLimiter.TryStart(_audioSource.clip) == false)
                return;

            _audioSource.Play();
        }
    }
}
