using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Weather
{
    // Дождь над полем — только отображение: капли, всплески на земле и звук. Сила дождя от 0 до 1 задаёт, сколько
    // частиц рождается и насколько громко звук; полная сила — то, что настроено в префабе. Силу меняет сервис погоды.
    public class RainView : MonoBehaviour
    {
        private const float NoRain = 0f;

        [SerializeField] private ParticleSystem _drops;
        [SerializeField] private ParticleSystem _splashes;
        [SerializeField] private AudioSource _sound;

        private float _fullDropsRate;
        private float _fullSplashesRate;
        private float _fullSoundVolume;

        public void SetIntensity(float intensity)
        {
            float clampedIntensity = Mathf.Clamp01(intensity);

            SetRate(_drops, _fullDropsRate * clampedIntensity);
            SetRate(_splashes, _fullSplashesRate * clampedIntensity);

            _sound.volume = _fullSoundVolume * clampedIntensity;

            if (clampedIntensity > NoRain)
                Play();
            else
                Stop();
        }

        private void Awake()
        {
            _fullDropsRate = _drops.emission.rateOverTimeMultiplier;
            _fullSplashesRate = _splashes.emission.rateOverTimeMultiplier;
            _fullSoundVolume = _sound.volume;

            // Длинная петля дождя без предзагрузки раскодировалась бы при первом дожде, посреди боя, — грузим её вместе
            // с уровнем
            _sound.clip.LoadAudioData();

            SetIntensity(NoRain);
        }

        private void Play()
        {
            if (_drops.isEmitting == false)
                _drops.Play(true);

            if (_splashes.isEmitting == false)
                _splashes.Play(true);

            if (_sound.isPlaying == false)
                _sound.Play();
        }

        // Капли, которые уже летят, долетают до земли и после остановки
        private void Stop()
        {
            _drops.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _splashes.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _sound.Stop();
        }

        private static void SetRate(ParticleSystem particleSystem, float rate)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = rate;
        }
    }
}
