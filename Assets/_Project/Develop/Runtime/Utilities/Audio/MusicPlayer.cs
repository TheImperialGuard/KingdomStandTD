using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void SetMusic(AudioClip clip) => _audioSource.clip = clip;

        public void Play() => _audioSource.Play();

        public void Stop() => _audioSource.Stop();
    }
}