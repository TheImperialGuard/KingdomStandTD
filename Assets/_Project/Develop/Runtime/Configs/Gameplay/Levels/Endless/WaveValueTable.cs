using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Число, которое меняется от волны к волне. Ключи «волна → значение» идут по возрастанию волн: между ключами значение
    // меняется линейно, до первого ключа равно значению первого, после последнего — по режиму хвоста. Пустая таблица —
    // ошибка конфига: инспектор режима показывает её, а в игре чтение бросает исключение, а не отдаёт ноль молча.
    [Serializable]
    public class WaveValueTable
    {
        [SerializeField] private List<WaveValueKey> _keys = new();

        [field: SerializeField] public WaveValueTailModes TailMode { get; private set; } = WaveValueTailModes.Hold;

        [field: Tooltip("Шаг хвоста: прибавка за волну (Linear) или доля роста за волну (Compound, 0.05 — +5 % за волну)")]
        [field: SerializeField] public float TailStep { get; private set; }

        public IReadOnlyList<WaveValueKey> Keys => _keys;

        public float GetValue(int waveNumber)
        {
            if (_keys.Count == 0)
                throw new InvalidOperationException("В таблице значений по волнам нет ни одного ключа");

            WaveValueKey firstKey = _keys[0];

            if (waveNumber <= firstKey.Wave)
                return firstKey.Value;

            for (int i = 1; i < _keys.Count; i++)
            {
                WaveValueKey key = _keys[i];

                if (waveNumber <= key.Wave)
                    return Interpolate(_keys[i - 1], key, waveNumber);
            }

            WaveValueKey lastKey = _keys[_keys.Count - 1];

            return GetTailValue(lastKey, waveNumber);
        }

        private float GetTailValue(WaveValueKey lastKey, int waveNumber)
        {
            int wavesAfterLastKey = waveNumber - lastKey.Wave;

            switch (TailMode)
            {
                case WaveValueTailModes.Hold:
                    return lastKey.Value;

                case WaveValueTailModes.Linear:
                    return lastKey.Value + TailStep * wavesAfterLastKey;

                case WaveValueTailModes.Compound:
                    return lastKey.Value * Mathf.Pow(1f + TailStep, wavesAfterLastKey);

                default:
                    throw new ArgumentOutOfRangeException(nameof(TailMode), TailMode, "Неизвестный режим хвоста таблицы");
            }
        }

        private static float Interpolate(WaveValueKey from, WaveValueKey to, int waveNumber)
        {
            float progress = Mathf.InverseLerp(from.Wave, to.Wave, waveNumber);

            return Mathf.Lerp(from.Value, to.Value, progress);
        }
    }
}
