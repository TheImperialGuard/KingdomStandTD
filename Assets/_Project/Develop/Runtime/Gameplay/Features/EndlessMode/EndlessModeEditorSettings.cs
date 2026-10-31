#if UNITY_EDITOR
using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Настройки бесконечного режима для проверок в редакторе. Хранятся в EditorPrefs, а не в ассете конфига: в ассете их
    // легко случайно закоммитить, а поле под #if UNITY_EDITOR поменяло бы раскладку сериализации между редактором и
    // сборкой. Показывают их инспекторы EndlessModeConfig и WeatherConfig, а игра обращается сюда только из блоков
    // #if UNITY_EDITOR — поэтому ни этот файл, ни обращения к нему в билд не попадают.
    public static class EndlessModeEditorSettings
    {
        private const string StartWaveNumberPrefsKey = "KingdomStandTD.EndlessMode.StartWaveNumber";
        private const string ForcedWeatherPrefsKey = "KingdomStandTD.EndlessMode.ForcedWeather";

        private const int NoForcedWeather = -1;

        // С какой волны начинается забег в редакторе; рекорд такого забега не засчитывается
        public static int StartWaveNumber
        {
            get => ClampWaveNumber(EditorPrefs.GetInt(StartWaveNumberPrefsKey, EndlessStagesSource.DefaultFirstWaveNumber));
            set => EditorPrefs.SetInt(StartWaveNumberPrefsKey, ClampWaveNumber(value));
        }

        // Погода, которая держится весь забег в редакторе; null — погода меняется по волнам, как в сборке
        public static WeatherKinds? ForcedWeather
        {
            get
            {
                int value = EditorPrefs.GetInt(ForcedWeatherPrefsKey, NoForcedWeather);

                if (Enum.IsDefined(typeof(WeatherKinds), value) == false)
                    return null;

                return (WeatherKinds)value;
            }

            set => EditorPrefs.SetInt(ForcedWeatherPrefsKey, value.HasValue ? (int)value.Value : NoForcedWeather);
        }

        private static int ClampWaveNumber(int waveNumber) => Mathf.Max(EndlessStagesSource.DefaultFirstWaveNumber, waveNumber);
    }
}
#endif
