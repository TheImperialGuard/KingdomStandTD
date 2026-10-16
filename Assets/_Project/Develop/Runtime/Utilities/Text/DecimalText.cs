using System.Globalization;

namespace Assets._Project.Develop.Runtime.Utilities.Text
{
    // Число для текста по-русски: дробная часть через запятую, не больше двух знаков, без лишних нулей — 2, 1,5, 0,75.
    public static class DecimalText
    {
        private const string TwoDecimalsFormat = "0.##";
        private const string DecimalSeparator = ",";

        private static readonly NumberFormatInfo _format = new() { NumberDecimalSeparator = DecimalSeparator };

        public static string Format(float value) => value.ToString(TwoDecimalsFormat, _format);
    }
}
