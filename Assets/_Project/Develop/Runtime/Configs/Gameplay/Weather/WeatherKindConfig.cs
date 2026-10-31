using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Weather
{
    // Одна погода: сколько волн она идёт наверняка, её свет, есть ли дождь и молнии.
    [Serializable]
    public class WeatherKindConfig
    {
        [SerializeField] private WeatherKinds _kind;

        [Tooltip("Сколько волн погода идёт наверняка; после них в начале каждой волны может смениться")]
        [SerializeField, Min(1)] private int _wavesCount = 2;

        [Tooltip("Свет погоды. Пусто — обычный свет уровня")]
        [SerializeField] private LevelLightingConfig _lighting;

        [SerializeField] private bool _hasRain;

        [SerializeField] private bool _hasLightning;

        public WeatherKinds Kind => _kind;
        public int WavesCount => _wavesCount;
        public LevelLightingConfig Lighting => _lighting;
        public bool HasRain => _hasRain;
        public bool HasLightning => _hasLightning;
    }
}
