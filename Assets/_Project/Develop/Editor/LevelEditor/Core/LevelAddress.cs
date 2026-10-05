using System.Collections.Generic;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Место в уровне для сообщений: «Стейдж 2, волна 1, группа 3». Индексы — с нуля, номера в тексте — с единицы.
    // Адрес без стейджа (волны из буфера обмена) тоже начинается с заглавной: «Волна 2, группа 1».
    public static class LevelAddress
    {
        public const int NO_INDEX = -1;

        private const string SEPARATOR = ", ";

        public static string Format(int stageIndex, int waveIndex = NO_INDEX, int groupIndex = NO_INDEX)
        {
            List<string> parts = new();

            AddPart(parts, "Стейдж", stageIndex);
            AddPart(parts, "волна", waveIndex);
            AddPart(parts, "группа", groupIndex);

            string address = string.Join(SEPARATOR, parts);

            return Capitalize(address);
        }

        private static void AddPart(List<string> parts, string label, int index)
        {
            if (index == NO_INDEX)
                return;

            int number = index + 1;
            parts.Add($"{label} {number}");
        }

        private static string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }
    }
}
