using System;
using System.Globalization;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Числа в срезах округляются до трёх знаков. Значения, совпадающие после округления, считаются равными:
    // иначе импорт неизменённого среза находил бы «правки» в хвостах float.
    public static class LevelSliceNumbers
    {
        public const int DECIMALS = 3;

        private const string TEXT_FORMAT = "0.###";

        public static double Round(double value) => Math.Round(value, DECIMALS, MidpointRounding.AwayFromZero);

        public static bool IsSame(float first, float second) => Round(first) == Round(second);

        public static string ToText(float value) => Round(value).ToString(TEXT_FORMAT, CultureInfo.InvariantCulture);
    }
}
