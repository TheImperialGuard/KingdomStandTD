using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Weather
{
    // Правила погоды боя: какие погоды бывают, сколько волн каждая идёт наверняка, с каким шансом потом сменяется и
    // какая смена запрещена. Ещё — переход между погодами.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Weather/NewWeatherConfig", fileName = "WeatherConfig")]
    public class WeatherConfig : ScriptableObject
    {
        [Tooltip("Погода первой волны")]
        [SerializeField] private WeatherKinds _startWeather = WeatherKinds.Clear;

        [SerializeField] private List<WeatherKindConfig> _weathers = new();

        [Tooltip("Шанс смены в начале первой волны после положенных погоде волн")]
        [SerializeField, Range(0f, 1f)] private float _changeChance = 0.25f;

        [Tooltip("Прибавка к шансу смены за каждую следующую волну без смены")]
        [SerializeField, Range(0f, 1f)] private float _changeChanceGrowth = 0.15f;

        [Tooltip("Запреты смены: после погоды From не бывает погоды To. Ту же самую погоду подряд не бывает и без запрета")]
        [SerializeField] private List<WeatherTransitionBan> _transitionBans = new();

        [Tooltip("За сколько секунд свет и дождь переходят от прежней погоды к новой")]
        [SerializeField, Min(0f)] private float _transitionDuration = 4f;

        public WeatherKinds StartWeather => _startWeather;
        public IReadOnlyList<WeatherKindConfig> Weathers => _weathers;
        public float ChangeChance => _changeChance;
        public float ChangeChanceGrowth => _changeChanceGrowth;
        public float TransitionDuration => _transitionDuration;

        public bool HasWeather(WeatherKinds kind) => TryFindWeather(kind, out _);

        public WeatherKindConfig GetWeather(WeatherKinds kind)
        {
            if (TryFindWeather(kind, out WeatherKindConfig weather) == false)
                throw new InvalidOperationException($"В конфиге погоды {name} нет погоды {kind}");

            return weather;
        }

        // Может ли погода next прийти на смену погоде previous
        public bool CanFollow(WeatherKinds previous, WeatherKinds next)
        {
            if (previous == next)
                return false;

            foreach (WeatherTransitionBan ban in _transitionBans)
            {
                if (ban.From == previous && ban.To == next)
                    return false;
            }

            return true;
        }

        private bool TryFindWeather(WeatherKinds kind, out WeatherKindConfig weather)
        {
            foreach (WeatherKindConfig candidate in _weathers)
            {
                if (candidate.Kind == kind)
                {
                    weather = candidate;
                    return true;
                }
            }

            weather = null;

            return false;
        }
    }
}
