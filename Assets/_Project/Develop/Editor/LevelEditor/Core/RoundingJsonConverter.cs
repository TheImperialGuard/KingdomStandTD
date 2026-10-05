using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Пишет float и double с округлением до заданного числа знаков. Бесконечность и NaN пишутся как null:
    // в JSON таких чисел нет, а бесконечный запас здоровья бывает у врага с полным сопротивлением.
    public class RoundingJsonConverter : JsonConverter
    {
        private readonly int _decimals;

        public RoundingJsonConverter(int decimals)
        {
            _decimals = decimals;
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(float)
                || objectType == typeof(double)
                || objectType == typeof(float?)
                || objectType == typeof(double?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            double number = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(Math.Round(number, _decimals, MidpointRounding.AwayFromZero));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotSupportedException();
    }
}
