using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Editor.LevelEditor;
using Assets._Project.Develop.Editor.LevelEditor.Window;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Инспектор EndlessModeConfig: поля конфига, итог проверки и таблица первых волн. Таблицу собирает тот же генератор,
    // что и в игре, по дорогам префаба уровня режима, поэтому она показывает ровно те волны, что выйдут в бою.
    // Пересчитывается при любой правке конфига; число волн в таблице — настройка этого инспектора, а не ассета.
    [CustomEditor(typeof(EndlessModeConfig))]
    public class EndlessModeConfigEditor : UnityEditor.Editor
    {
        private const string ScriptProperty = "m_Script";
        private const string WavesCountPrefsKey = "KingdomStandTD.EndlessModeConfigEditor.WavesCount";
        private const string TimeFormat = "0.0";
        private const string IssuesSeparator = "\n";

        private const int FirstWaveNumber = 1;
        private const int DefaultWavesCount = 40;
        private const int MinWavesCount = 1;
        private const int MaxWavesCount = 300;

        private const float SectionMarginTop = 10f;
        private const float WaveColumnWidth = 44f;
        private const float CountColumnWidth = 52f;
        private const float TierColumnWidth = 64f;
        private const float TimeColumnWidth = 60f;
        private const float CellPadding = 2f;

        private static readonly Color HeaderBackground = new(0f, 0f, 0f, 0.2f);
        private static readonly Color EvenRowBackground = new(1f, 1f, 1f, 0.04f);

        private HelpBox _issuesBox;
        private VisualElement _table;
        private int _wavesCount;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            PropertyField scriptField = new(serializedObject.FindProperty(ScriptProperty));
            scriptField.SetEnabled(false);

            root.Add(scriptField);

            foreach (SerializedProperty property in EnumerateConfigProperties())
                root.Add(new PropertyField(property));

            root.Add(CreateWavesSection());
            root.TrackSerializedObjectValue(serializedObject, _ => Refresh());

            Refresh();

            return root;
        }

        private VisualElement CreateWavesSection()
        {
            VisualElement section = new();
            section.style.marginTop = SectionMarginTop;

            Label title = new("Волны режима");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;

            _wavesCount = Mathf.Clamp(EditorPrefs.GetInt(WavesCountPrefsKey, DefaultWavesCount), MinWavesCount, MaxWavesCount);

            IntegerField wavesCountField = new("Волн в таблице") { value = _wavesCount };
            wavesCountField.RegisterValueChangedCallback(changeEvent => OnWavesCountChanged(wavesCountField, changeEvent.newValue));

            _issuesBox = new HelpBox(string.Empty, HelpBoxMessageType.None);
            _table = new VisualElement();

            section.Add(title);
            section.Add(_issuesBox);
            section.Add(wavesCountField);
            section.Add(_table);

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

        private void OnWavesCountChanged(IntegerField field, int wavesCount)
        {
            int clampedCount = Mathf.Clamp(wavesCount, MinWavesCount, MaxWavesCount);

            if (clampedCount != wavesCount)
                field.SetValueWithoutNotify(clampedCount);

            _wavesCount = clampedCount;
            EditorPrefs.SetInt(WavesCountPrefsKey, clampedCount);

            Refresh();
        }

        private void Refresh()
        {
            EndlessModeConfig config = (EndlessModeConfig)target;

            if (config == null)
                return;

            Level levelPrefab = LoadLevelPrefab(config);
            List<LevelIssue> issues = EndlessModeConfigValidator.Validate(config, levelPrefab);

            ShowIssues(issues);

            _table.Clear();

            if (issues.Any(issue => issue.IsError))
            {
                _table.Add(new HelpBox("Таблица появится, когда в конфиге не останется ошибок", HelpBoxMessageType.Info));
                return;
            }

            EndlessWavesGenerator generator = new(config, levelPrefab.RoadPaths);

            _table.Add(CreateHeaderRow(config));

            for (int waveNumber = FirstWaveNumber; waveNumber < FirstWaveNumber + _wavesCount; waveNumber++)
            {
                EnemiesWavesStageConfig wave = generator.Generate(waveNumber);
                EndlessWaveSummary summary = EndlessWaveSummary.Create(waveNumber, wave, config);

                _table.Add(CreateWaveRow(summary));
            }
        }

        private void ShowIssues(List<LevelIssue> issues)
        {
            if (issues.Count == 0)
            {
                _issuesBox.text = "Замечаний нет";
                _issuesBox.messageType = HelpBoxMessageType.Info;
                return;
            }

            _issuesBox.text = string.Join(IssuesSeparator, issues.Select(LevelEditorTexts.FormatIssue));
            _issuesBox.messageType = issues.Any(issue => issue.IsError) ? HelpBoxMessageType.Error : HelpBoxMessageType.Warning;
        }

        private VisualElement CreateHeaderRow(EndlessModeConfig config)
        {
            VisualElement row = CreateRow(HeaderBackground);

            AddCell(row, "Волна", WaveColumnWidth);
            AddCell(row, "Врагов", CountColumnWidth);

            foreach (EndlessTier tier in config.Tiers)
                AddCell(row, tier.Name, TierColumnWidth);

            AddCompositionCell(row, "Состав");
            AddCell(row, "Спавн, с", TimeColumnWidth);
            AddCell(row, "Волна, с", TimeColumnWidth);

            row.style.unityFontStyleAndWeight = FontStyle.Bold;

            return row;
        }

        private VisualElement CreateWaveRow(EndlessWaveSummary summary)
        {
            bool isEven = summary.WaveNumber % 2 == 0;
            VisualElement row = CreateRow(isEven ? EvenRowBackground : Color.clear);

            AddCell(row, summary.WaveNumber.ToString(), WaveColumnWidth);
            AddCell(row, summary.EnemiesCount.ToString(), CountColumnWidth);

            foreach (int tierEnemiesCount in summary.TiersEnemiesCounts)
                AddCell(row, tierEnemiesCount.ToString(), TierColumnWidth);

            AddCompositionCell(row, summary.Composition);
            AddCell(row, summary.SpawnDuration.ToString(TimeFormat), TimeColumnWidth);
            AddCell(row, summary.WaveDuration.ToString(TimeFormat), TimeColumnWidth);

            return row;
        }

        private static VisualElement CreateRow(Color background)
        {
            VisualElement row = new();

            row.style.flexDirection = FlexDirection.Row;
            row.style.backgroundColor = background;

            return row;
        }

        private static void AddCell(VisualElement row, string text, float width)
        {
            Label cell = new(text);

            cell.style.width = width;
            cell.style.flexShrink = 0;
            cell.style.paddingLeft = CellPadding;
            cell.style.paddingRight = CellPadding;

            row.Add(cell);
        }

        private static void AddCompositionCell(VisualElement row, string text)
        {
            Label cell = new(text);

            cell.style.flexGrow = 1;
            cell.style.flexShrink = 1;
            cell.style.whiteSpace = WhiteSpace.Normal;
            cell.style.paddingLeft = CellPadding;
            cell.style.paddingRight = CellPadding;

            row.Add(cell);
        }

        // Префаб так же, как его грузит игра: Resources.Load по LevelConfig.PrefabPath.
        private static Level LoadLevelPrefab(EndlessModeConfig config)
        {
            if (config.Level == null || string.IsNullOrWhiteSpace(config.Level.PrefabPath))
                return null;

            return Resources.Load<Level>(config.Level.PrefabPath);
        }
    }
}
