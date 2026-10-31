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
    // погоды. Пока свет не меняется, сцену не трогает.
    public class WeatherLighting : IInitializable
    {
        private readonly LevelLightingConfig _levelLighting;
        private readonly Light _sun;
        private readonly WeatherConfig _config;

        private readonly Dictionary<WeatherKinds, SceneLighting> _weathersLighting = new();

        private SceneLighting _current;
        private bool _isChanged;

        // levelLighting — свет уровня, null — у уровня свой свет не задан и горит свет сцены
        public WeatherLighting(LevelLightingConfig levelLighting, Light sun, WeatherConfig config)
        {
            _levelLighting = levelLighting;
            _sun = sun;
            _config = config;
        }

        // Свет, который сейчас стоит на сцене
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

        public void Update(float deltaTime)
        {
            if (_isChanged == false)
                return;

            _isChanged = false;

            _current.ApplyTo(_sun);
        }

        // Свет пресета так, как его ставит LevelLightingService: гармоники окружения из градиента считает сам Unity
        private SceneLighting Measure(LevelLightingConfig lighting)
        {
            LevelLightingService.Apply(lighting, _sun);

            return SceneLighting.Capture(_sun);
        }
    }
}
