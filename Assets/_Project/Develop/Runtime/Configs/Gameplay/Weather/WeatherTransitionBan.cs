using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Weather
{
    // Запрет смены погоды: после погоды From не бывает погоды To.
    [Serializable]
    public class WeatherTransitionBan
    {
        [SerializeField] private WeatherKinds _from;

        [SerializeField] private WeatherKinds _to;

        public WeatherKinds From => _from;
        public WeatherKinds To => _to;
    }
}
