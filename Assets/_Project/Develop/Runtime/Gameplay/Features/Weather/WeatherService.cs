using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;
#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
#endif

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Weather
{
    // Погода боя. На старте ставит стартовую погоду сразу, без перехода. В начале каждой волны делает шаг
    // последовательности погод; если погода сменилась — за время перехода плавно переводит к ней свет и силу дождя, а
    // молнии включает или выключает сразу. Пока погода задана в редакторе принудительно, она не меняется.
    public class WeatherService : IInitializable, IDisposable
    {
        private const float NoRain = 0f;
        private const float FullRain = 1f;
        private const float TransitionEnd = 1f;

        private readonly WeatherConfig _config;
        private readonly WeatherSequence _sequence;
        private readonly WeatherLighting _lighting;
        private readonly RainView _rainView;
        private readonly LightningStrikesService _lightningStrikesService;
        private readonly StageProviderService _stageProviderService;
        private readonly bool _isWeatherForced;

        private IDisposable _stageNumberDisposable;

        private WeatherKindConfig _weather;

        private SceneLighting _fromLighting;
        private float _fromRainIntensity;
        private float _rainIntensity;

        private float _transitionElapsedTime;
        private bool _isInTransition;

        // isWeatherForced — погода задана в редакторе и держится весь бой
        public WeatherService(
            WeatherConfig config,
            WeatherSequence sequence,
            WeatherLighting lighting,
            RainView rainView,
            LightningStrikesService lightningStrikesService,
            StageProviderService stageProviderService,
            bool isWeatherForced)
        {
            _config = config;
            _sequence = sequence;
            _lighting = lighting;
            _rainView = rainView;
            _lightningStrikesService = lightningStrikesService;
            _stageProviderService = stageProviderService;
            _isWeatherForced = isWeatherForced;
        }

        public void Initialize()
        {
            _weather = _config.GetWeather(_sequence.Current);

            _lighting.Set(_lighting.GetLighting(_weather.Kind));
            SetRainIntensity(GetRainIntensity(_weather));
            _lightningStrikesService.SetActive(_weather.HasLightning);

            _stageNumberDisposable = _stageProviderService.CurrentStageNumber.Subscribe(OnStageNumberChanged);

            Debug.Log($"Погода на старте: {_weather.Kind}");
        }

        public void Update(float deltaTime)
        {
            UpdateTransition(deltaTime);

            _lighting.Update(deltaTime);
            _lightningStrikesService.Update(deltaTime);
        }

        public void Dispose()
        {
            _stageNumberDisposable?.Dispose();
        }

        private void OnStageNumberChanged(int oldNumber, int newNumber)
        {
            TryChangeWeather(newNumber);

            #if UNITY_EDITOR
                // Плейтест из редактора уровней пишет погоду каждой волны в лог прогона
                PlaytestHooks.NotifyWaveWeather(_weather.Kind, newNumber);
            #endif
        }

        private void TryChangeWeather(int waveNumber)
        {
            if (_isWeatherForced)
                return;

            if (_sequence.TryChangeOnWaveStart() == false)
                return;

            StartTransition(_config.GetWeather(_sequence.Current));

            Debug.Log($"Волна {waveNumber}: погода сменилась на {_weather.Kind}");
        }

        // Переход начинается с того света и дождя, что сейчас на сцене, даже если прошлый переход не закончился
        private void StartTransition(WeatherKindConfig weather)
        {
            _weather = weather;

            _fromLighting = _lighting.Current;
            _fromRainIntensity = _rainIntensity;

            _transitionElapsedTime = 0f;
            _isInTransition = true;

            _lightningStrikesService.SetActive(weather.HasLightning);
        }

        private void UpdateTransition(float deltaTime)
        {
            if (_isInTransition == false)
                return;

            _transitionElapsedTime += deltaTime;

            float progress = GetTransitionProgress();

            _lighting.Set(SceneLighting.Lerp(_fromLighting, _lighting.GetLighting(_weather.Kind), progress));
            SetRainIntensity(Mathf.Lerp(_fromRainIntensity, GetRainIntensity(_weather), progress));

            if (progress >= TransitionEnd)
                _isInTransition = false;
        }

        private float GetTransitionProgress()
        {
            if (_config.TransitionDuration <= 0f)
                return TransitionEnd;

            return Mathf.Clamp01(_transitionElapsedTime / _config.TransitionDuration);
        }

        private void SetRainIntensity(float intensity)
        {
            _rainIntensity = intensity;
            _rainView.SetIntensity(intensity);
        }

        private static float GetRainIntensity(WeatherKindConfig weather) => weather.HasRain ? FullRain : NoRain;
    }
}
