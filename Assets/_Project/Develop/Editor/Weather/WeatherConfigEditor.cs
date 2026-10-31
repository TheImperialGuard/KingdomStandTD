using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.Weather
{
    // Инспектор WeatherConfig: поля конфига, замечания к нему и принудительная погода для проверок в редакторе. Она
    // хранится в EditorPrefs, а не в ассете; игра берёт её через EndlessModeEditorSettings.
    [CustomEditor(typeof(WeatherConfig))]
    public class WeatherConfigEditor : UnityEditor.Editor
    {
        private const string ScriptProperty = "m_Script";
        private const string NoForcedWeatherChoice = "Нет — меняется по волнам";
        private const string IssuesSeparator = "\n";

        private const int NoForcedWeatherIndex = 0;
        private const int FirstWeatherIndex = 1;
        private const float SectionMarginTop = 10f;

        private HelpBox _forcedWeatherBox;
        private HelpBox _issuesBox;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            PropertyField scriptField = new(serializedObject.FindProperty(ScriptProperty));
            scriptField.SetEnabled(false);

            root.Add(scriptField);

            _issuesBox = new HelpBox(string.Empty, HelpBoxMessageType.None);
            root.Add(_issuesBox);

            foreach (SerializedProperty property in EnumerateConfigProperties())
                root.Add(new PropertyField(property));

            root.Add(CreateEditorChecksSection());
            root.TrackSerializedObjectValue(serializedObject, _ => ShowIssues());

            ShowIssues();

            return root;
        }

        private VisualElement CreateEditorChecksSection()
        {
            VisualElement section = new();
            section.style.marginTop = SectionMarginTop;

            Label title = new("Проверки в редакторе");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;

            List<string> choices = new() { NoForcedWeatherChoice };
            choices.AddRange(Enum.GetNames(typeof(WeatherKinds)));

            DropdownField forcedWeatherField = new("Принудительная погода", choices, GetForcedWeatherIndex());
            forcedWeatherField.RegisterValueChangedCallback(_ => OnForcedWeatherChanged(forcedWeatherField.index));

            _forcedWeatherBox = new HelpBox(string.Empty, HelpBoxMessageType.Warning);

            section.Add(title);
            section.Add(forcedWeatherField);
            section.Add(_forcedWeatherBox);

            UpdateForcedWeatherBox();

            return section;
        }

        // Видимые поля конфига, кроме скрипта.
        private IEnumerable<SerializedProperty> EnumerateConfigProperties()
        {
            SerializedProperty property = serializedObject.GetIterator();

            for (bool enterChildren = true; property.NextVisible(enterChildren); enterChildren = false)
            {
                if (property.propertyPath == ScriptProperty)
                    continue;

                yield return property.Copy();
            }
        }

        // Пункт 0 — «нет», дальше погоды в порядке перечисления
        private static int GetForcedWeatherIndex()
        {
            WeatherKinds? forcedWeather = EndlessModeEditorSettings.ForcedWeather;

            return forcedWeather.HasValue ? (int)forcedWeather.Value + FirstWeatherIndex : NoForcedWeatherIndex;
        }

        private void OnForcedWeatherChanged(int index)
        {
            EndlessModeEditorSettings.ForcedWeather = index == NoForcedWeatherIndex ? null : (WeatherKinds)(index - FirstWeatherIndex);

            UpdateForcedWeatherBox();
            ShowIssues();
        }

        // Предупреждение видно, только пока погода задана принудительно
        private void UpdateForcedWeatherBox()
        {
            WeatherKinds? forcedWeather = EndlessModeEditorSettings.ForcedWeather;

            _forcedWeatherBox.text = $"Забег в редакторе пройдёт в погоде {forcedWeather} с первой минуты, смены погоды не будет. "
                + "В сборке погода всегда меняется по волнам";
            _forcedWeatherBox.style.display = forcedWeather.HasValue ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ShowIssues()
        {
            WeatherConfig config = (WeatherConfig)target;

            if (config == null)
                return;

            List<string> issues = CollectIssues(config);

            if (issues.Count == 0)
            {
                _issuesBox.text = "Замечаний нет";
                _issuesBox.messageType = HelpBoxMessageType.Info;
                return;
            }

            _issuesBox.text = string.Join(IssuesSeparator, issues);
            _issuesBox.messageType = HelpBoxMessageType.Warning;
        }

        private static List<string> CollectIssues(WeatherConfig config)
        {
            List<string> issues = new();

            CheckWeatherExists(config, config.StartWeather, "Стартовой погоды", issues);

            WeatherKinds? forcedWeather = EndlessModeEditorSettings.ForcedWeather;

            if (forcedWeather.HasValue)
                CheckWeatherExists(config, forcedWeather.Value, "Принудительной погоды", issues);

            CheckWeathers(config, issues);

            if (config.RainPrefab == null)
                issues.Add("Не задан префаб дождя — бой с погодой не запустится");

            return issues;
        }

        private static void CheckWeatherExists(WeatherConfig config, WeatherKinds kind, string role, List<string> issues)
        {
            if (config.HasWeather(kind) == false)
                issues.Add($"{role} {kind} нет в списке погод — бой с погодой не запустится");
        }

        private static void CheckWeathers(WeatherConfig config, List<string> issues)
        {
            HashSet<WeatherKinds> kinds = new();

            foreach (WeatherKindConfig weather in config.Weathers)
            {
                if (kinds.Add(weather.Kind) == false)
                    issues.Add($"Погода {weather.Kind} в списке дважды — действует первая");

                if (HasNextWeather(config, weather.Kind) == false)
                    issues.Add($"После погоды {weather.Kind} сменить её не на что — она останется навсегда");
            }
        }

        private static bool HasNextWeather(WeatherConfig config, WeatherKinds kind)
        {
            foreach (WeatherKindConfig weather in config.Weathers)
            {
                if (config.CanFollow(kind, weather.Kind))
                    return true;
            }

            return false;
        }
    }
}
