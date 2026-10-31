#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Настройки бесконечного режима для проверок в редакторе. Хранятся в EditorPrefs, а не в ассете конфига: в ассете их
    // легко случайно закоммитить, а поле под #if UNITY_EDITOR поменяло бы раскладку сериализации между редактором и
    // сборкой. Показывает их инспектор EndlessModeConfig, а игра обращается сюда только из блоков #if UNITY_EDITOR —
    // поэтому ни этот файл, ни обращения к нему в билд не попадают.
    public static class EndlessModeEditorSettings
    {
        private const string StartWaveNumberPrefsKey = "KingdomStandTD.EndlessMode.StartWaveNumber";

        // С какой волны начинается забег в редакторе; рекорд такого забега не засчитывается
        public static int StartWaveNumber
        {
            get => ClampWaveNumber(EditorPrefs.GetInt(StartWaveNumberPrefsKey, EndlessStagesSource.DefaultFirstWaveNumber));
            set => EditorPrefs.SetInt(StartWaveNumberPrefsKey, ClampWaveNumber(value));
        }

        private static int ClampWaveNumber(int waveNumber) => Mathf.Max(EndlessStagesSource.DefaultFirstWaveNumber, waveNumber);
    }
}
#endif
