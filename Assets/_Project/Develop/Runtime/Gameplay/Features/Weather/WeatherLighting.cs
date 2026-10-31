using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Weather
{
    // Свет погоды. На старте снимает свет каждой погоды: обычный — свет уровня, если он задан, иначе свет сцены;
    // пресет погоды — так, как его поставил бы LevelLightingService. Дальше ставит сцене свет, который задаёт сервис
    // погоды, и вспышку молнии поверх него. Пока свет не меняется и вспышки нет, сцену не трогает.
    public class WeatherLighting : IInitializable
    {
        private readonly LevelLightingConfig _levelLighting;
        private readonly Light _sun;
        private readonly WeatherConfig _config;

        private readonly Dictionary<WeatherKinds, SceneLighting> _weathersLighting = new();

        private SceneLighting _current;
        private bool _isChanged;

        private float _flashIntensity;
        private float _flashDuration;
        private float _flashRemainingTime;

        // levelLighting — свет уровня, null — у уровня свой свет не задан и горит свет сцены
        public WeatherLighting(LevelLightingConfig levelLighting, Light sun, WeatherConfig config)
        {
            _levelLighting = levelLighting;
            _sun = sun;
            _config = config;
        }

        // Свет, который сейчас стоит на сцене, без вспышки
        public SceneLighting Current => _current;

        public void Initialize()
        {
            SceneLighting sceneLighting = SceneLighting.Capture(_sun);
            SceneLighting levelLighting = _levelLighting == null ? sceneLighting : Measure(_levelLighting);

            foreach (WeatherKindConfig weather in _config.Weathers)
                _weathersLighting[weather.Kind] = weather.Lighting == null ? levelLighting : Measure(weather.Lighting);

            // Замеры ставили сцене свет пресетов — вернуть прежний
            sceneLighting.ApplyTo(_sun);

            _current = sceneLighting;
        }

        public SceneLighting GetLighting(WeatherKinds kind) => _weathersLighting[kind];

        public void Set(SceneLighting lighting)
        {
            _current = lighting;
            _isChanged = true;
        }

        // Солнце вспыхивает ярче на intensity и за duration секунд гаснет до обычного
        public void Flash(float intensity, float duration)
        {
            _flashIntensity = intensity;
            _flashDuration = duration;
            _flashRemainingTime = duration;
            _isChanged = true;
        }

        public void Update(float deltaTime)
        {
            bool isFlashing = _flashRemainingTime > 0f;

            if (isFlashing)
                _flashRemainingTime = Mathf.Max(0f, _flashRemainingTime - deltaTime);

            if (_isChanged == false && isFlashing == false)
                return;

            _isChanged = false;

            _current.WithSunIntensity(_current.SunIntensity + GetFlashIntensity()).ApplyTo(_sun);
        }

        private float GetFlashIntensity()
        {
            if (_flashDuration <= 0f)
                return 0f;

            return _flashIntensity * _flashRemainingTime / _flashDuration;
        }

        // Свет пресета так, как его ставит LevelLightingService: гармоники окружения из градиента считает сам Unity
        private SceneLighting Measure(LevelLightingConfig lighting)
        {
            LevelLightingService.Apply(lighting, _sun);

            return SceneLighting.Capture(_sun);
        }
    }
}
