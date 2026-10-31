using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using UnityEngine;
using Random = System.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Weather
{
    // Смена погоды по волнам. Погода идёт положенное ей число волн, затем в начале каждой волны сменяется с шансом,
    // который растёт за каждую волну без смены, пока не дойдёт до гарантии. Новая погода выбирается поровну из
    // разрешённых: не та же самая и не запрещённая после текущей. Новая погода начинает свой счёт волн с нуля.
    public class WeatherSequence
    {
        private const int FirstWaveOfWeather = 1;
        private const float GuaranteedChance = 1f;

        private readonly WeatherConfig _config;
        private readonly Random _random;
        private readonly List<WeatherKinds> _candidates = new();

        private WeatherKinds _current;

        // Сколько волн идёт текущая погода, считая текущую волну; до первой волны — 0
        private int _wavesCount;

        public WeatherSequence(WeatherConfig config, Random random, WeatherKinds startWeather)
        {
            _config = config;
            _random = random;
            _current = startWeather;
        }

        public WeatherKinds Current => _current;
        public int WavesCount => _wavesCount;

        // Шанс, что погода сменится в начале следующей волны
        public float ChangeChance
        {
            get
            {
                int guaranteedWavesCount = _config.GetWeather(_current).WavesCount;

                if (_wavesCount < guaranteedWavesCount)
                    return 0f;

                int extraWavesCount = _wavesCount - guaranteedWavesCount;
                float chance = _config.ChangeChance + _config.ChangeChanceGrowth * extraWavesCount;

                return Mathf.Min(chance, GuaranteedChance);
            }
        }

        // Начало волны: погода либо идёт дальше, либо сменяется. true — сменилась
        public bool TryChangeOnWaveStart()
        {
            if (IsChangeRolled() && TryPickNext(out WeatherKinds next))
            {
                _current = next;
                _wavesCount = FirstWaveOfWeather;

                return true;
            }

            _wavesCount++;

            return false;
        }

        private bool IsChangeRolled() => _random.NextDouble() < ChangeChance;

        private bool TryPickNext(out WeatherKinds next)
        {
            _candidates.Clear();

            foreach (WeatherKindConfig weather in _config.Weathers)
            {
                if (_config.CanFollow(_current, weather.Kind) && _candidates.Contains(weather.Kind) == false)
                    _candidates.Add(weather.Kind);
            }

            next = _current;

            if (_candidates.Count == 0)
                return false;

            next = _candidates[_random.Next(_candidates.Count)];

            return true;
        }
    }
}
