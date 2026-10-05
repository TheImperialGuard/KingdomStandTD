using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Editor.LevelEditor.Window;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Inspector
{
    // Инспектор LevelConfig. Стейджи и волны правятся в редакторе уровней, поэтому вместо их сырого списка —
    // кнопка, открывающая уровень в редакторе, сводка по уровню и итог проверки. Остальные поля — как обычно.
    // Сводка пересчитывается при любой правке конфига: в инспекторе, в редакторе уровней или через Undo.
    [CustomEditor(typeof(LevelConfig))]
    [CanEditMultipleObjects]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        private const string STYLES_FILE_NAME = "LevelConfigEditor.uss";
        private const string SCRIPT_PROPERTY = "m_Script";
        private const string SUMMARY_CLASS = "level-config-inspector__summary";
        private const string OPEN_BUTTON_CLASS = "level-config-inspector__open-button";

        private GameConfig _gameConfig;
        private HelpBox _totalsBox;
        private HelpBox _issuesBox;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            StyleSheet styles = LevelEditorWindowFiles.LoadNextToScript<StyleSheet>(this, STYLES_FILE_NAME);

            if (styles != null)
                root.styleSheets.Add(styles);

            PropertyField scriptField = new(serializedObject.FindProperty(SCRIPT_PROPERTY));
            scriptField.SetEnabled(false);

            root.Add(scriptField);
            root.Add(CreateSummary());

            foreach (SerializedProperty property in EnumerateConfigProperties())
                root.Add(new PropertyField(property));

            return root;
        }

        private VisualElement CreateSummary()
        {
            VisualElement summary = new();
            summary.AddToClassList(SUMMARY_CLASS);

            if (targets.Length > 1)
            {
                int levelsCount = targets.Length;
                string text = $"Выбрано уровней: {levelsCount}. Сводка и переход в редактор уровней — при выборе одного уровня";

                summary.Add(new HelpBox(text, HelpBoxMessageType.Info));
                return summary;
            }

            Button openButton = new(OpenInLevelEditor)
            {
                text = "Открыть в редакторе уровней",
                tooltip = "Стейджи и волны правятся в редакторе уровней. Он же открывается двойным щелчком по уровню в окне Project",
            };

            openButton.AddToClassList(OPEN_BUTTON_CLASS);

            _gameConfig = LevelEditorAssets.FindGameConfig();
            _totalsBox = new HelpBox(string.Empty, HelpBoxMessageType.None);
            _issuesBox = new HelpBox(string.Empty, HelpBoxMessageType.None);

            summary.Add(openButton);
            summary.Add(_totalsBox);
            summary.Add(_issuesBox);
            summary.TrackSerializedObjectValue(serializedObject, _ => RefreshSummary());

            RefreshSummary();

            return summary;
        }

        // Видимые поля конфига, кроме скрипта и списка стейджей.
        private IEnumerable<SerializedProperty> EnumerateConfigProperties()
        {
            SerializedProperty property = serializedObject.GetIterator();

            for (bool enterChildren = true; property.NextVisible(enterChildren); enterChildren = false)
            {
                if (property.propertyPath == SCRIPT_PROPERTY || property.propertyPath == LevelConfigFields.STAGES)
                    continue;

                yield return property.Copy();
            }
        }

        private void RefreshSummary()
        {
            LevelConfig config = (LevelConfig)target;

            if (config == null)
                return;

            LevelPrefabInfo prefab = LevelPrefabReader.Read(config);
            LevelStats stats = LevelStatsCalculator.Calculate(config, prefab, _gameConfig);
            IReadOnlyList<LevelIssue> issues = LevelValidator.Validate(config, prefab);

            _totalsBox.text = LevelEditorTexts.DescribeLevelTotals(config, stats);
            _issuesBox.text = LevelEditorTexts.DescribeIssuesCount(issues);
            _issuesBox.messageType = GetIssuesMessageType(issues);
        }

        private static HelpBoxMessageType GetIssuesMessageType(IReadOnlyList<LevelIssue> issues)
        {
            if (issues.Any(issue => issue.IsError))
                return HelpBoxMessageType.Error;

            return issues.Count > 0 ? HelpBoxMessageType.Warning : HelpBoxMessageType.Info;
        }

        private void OpenInLevelEditor() => LevelEditorWindow.Open((LevelConfig)target);
    }
}
