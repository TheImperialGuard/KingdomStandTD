using System;
using System.Collections.Generic;
using System.Globalization;
using Assets._Project.Develop.Editor.LevelEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Человекочитаемый дифф настроек бесконечного режима: одна строка — одно изменённое значение, адрес — путь в блоке
    // endless среза (endless.tiers[1].share.keys[0].value). Списки сравниваются по позициям; лишние в конце —
    // добавленные или удалённые элементы.
    public static class EndlessSettingsComparer
    {
        private const string RootPath = "endless";
        private const string Arrow = " → ";
        private const string EmptyValue = "пусто";

        public static List<string> Compare(EndlessSettingsSlice before, EndlessSettingsSlice after)
        {
            List<string> changes = new();

            CompareTokens(changes, RootPath, LevelSliceSerializer.ToToken(before), LevelSliceSerializer.ToToken(after));

            return changes;
        }

        private static void CompareTokens(List<string> changes, string path, JToken before, JToken after)
        {
            if (before is JObject beforeObject && after is JObject afterObject)
            {
                foreach (JProperty property in beforeObject.Properties())
                    CompareTokens(changes, $"{path}.{property.Name}", property.Value, afterObject[property.Name]);

                return;
            }

            if (before is JArray beforeArray && after is JArray afterArray)
            {
                CompareArrays(changes, path, beforeArray, afterArray);
                return;
            }

            if (JToken.DeepEquals(before, after))
                return;

            string beforeText = Describe(before);
            string afterText = Describe(after);
            changes.Add($"{path}: {beforeText}{Arrow}{afterText}");
        }

        private static void CompareArrays(List<string> changes, string path, JArray before, JArray after)
        {
            int commonCount = Math.Min(before.Count, after.Count);

            for (int i = 0; i < commonCount; i++)
                CompareTokens(changes, $"{path}[{i}]", before[i], after[i]);

            for (int i = commonCount; i < after.Count; i++)
            {
                string itemText = Describe(after[i]);
                changes.Add($"{path}[{i}]: добавлен — {itemText}");
            }

            for (int i = commonCount; i < before.Count; i++)
            {
                string itemText = Describe(before[i]);
                changes.Add($"{path}[{i}]: удалён — {itemText}");
            }
        }

        private static string Describe(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return EmptyValue;

            if (token is JValue value)
                return Convert.ToString(value.Value, CultureInfo.InvariantCulture);

            return token.ToString(Formatting.None);
        }
    }
}
