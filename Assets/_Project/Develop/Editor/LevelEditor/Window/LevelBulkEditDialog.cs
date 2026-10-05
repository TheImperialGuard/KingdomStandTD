using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Массовые операции над стейджами уровня с предпросмотром: количество врагов и интервал спавна — в процентах,
    // старт волн, время стейджа и скип — сдвигом в секундах. Область — стейджи с N по M; кнопки рядом ставят выбранный
    // стейдж или весь уровень. Окно модальное и уровень не меняет: при каждом изменении поля операция заново
    // применяется к свежему срезу уровня, а предпросмотр считает тот же код, что у вставки уровня из буфера.
    public class LevelBulkEditDialog : EditorWindow
    {
        private const string WINDOW_TITLE = "Массовые операции";
        private const string STYLE_FILE_NAME = "LevelEditorWindow.uss";
        private const float WINDOW_WIDTH = 640f;
        private const float WINDOW_HEIGHT = 600f;

        // Окно можно растягивать; меньше этого поля и кнопки области перестают помещаться.
        private const float MIN_WINDOW_WIDTH = 520f;
        private const float MIN_WINDOW_HEIGHT = 380f;

        private const int FIRST_STAGE_NUMBER = 1;
        private const int MIN_PERCENT = 1;

        private const string RANGE_CLASS = "bulk-edit__range";
        private const string RANGE_LABEL_CLASS = "bulk-edit__range-label";
        private const string STAGE_FIELD_CLASS = "bulk-edit__stage-field";
        private const string FIELDS_CLASS = "bulk-edit__fields";
        private const string FIELD_CLASS = "bulk-edit__field";
        private const string OPERATIONS_SEPARATOR = ", ";

        private LevelConfig _config;
        private LevelSliceEnemies _enemies;
        private int _selectedStageIndex;
        private Action<LevelSlice, LevelBulkEditSettings> _applied;

        private IntegerField _firstStageField;
        private IntegerField _lastStageField;
        private IntegerField _countField;
        private IntegerField _delayField;
        private FloatField _startShiftField;
        private FloatField _stageTimeShiftField;
        private FloatField _skipShiftField;
        private ScrollView _content;
        private Label _footer;
        private Button _applyButton;

        private LevelBulkEditSettings _settings;
        private LevelSlice _editedSlice;

        // Показывает окно и ждёт решения. true — операцию применили: срез уровня после неё и описание для шага Undo.
        public static bool TryEdit(
            LevelConfig config,
            LevelSliceEnemies enemies,
            int selectedStageIndex,
            out LevelSlice editedSlice,
            out string description)
        {
            LevelSlice appliedSlice = null;
            LevelBulkEditSettings appliedSettings = null;
            LevelBulkEditDialog dialog = CreateInstance<LevelBulkEditDialog>();

            dialog._config = config;
            dialog._enemies = enemies;
            dialog._selectedStageIndex = selectedStageIndex;
            dialog._applied = (slice, settings) =>
            {
                appliedSlice = slice;
                appliedSettings = settings;
            };

            dialog.titleContent = new GUIContent(WINDOW_TITLE);
            dialog.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
            dialog.position = DialogElements.GetCenteredRect(WINDOW_WIDTH, WINDOW_HEIGHT);
            dialog.ShowModalUtility();

            editedSlice = appliedSlice;
            description = appliedSlice != null ? DescribeSettings(appliedSettings, config.Stages.Count) : null;

            return appliedSlice != null;
        }

        // «стейджи 2–4: количество 120 %, старт -2 с» — имя шага Undo и текст уведомления.
        private static string DescribeSettings(LevelBulkEditSettings settings, int stagesCount)
        {
            List<string> operations = new();

            if (settings.CountPercent != LevelBulkEditSettings.UNCHANGED_PERCENT)
                operations.Add($"количество {settings.CountPercent} %");

            if (settings.DelayPercent != LevelBulkEditSettings.UNCHANGED_PERCENT)
                operations.Add($"интервал {settings.DelayPercent} %");

            if (settings.StartShift != 0f)
                operations.Add($"старт {FormatShift(settings.StartShift)}");

            if (settings.StageTimeShift != 0f)
                operations.Add($"время стейджа {FormatShift(settings.StageTimeShift)}");

            if (settings.SkipShift != 0f)
                operations.Add($"скип {FormatShift(settings.SkipShift)}");

            string scope = DescribeScope(settings, stagesCount);
            string operationsText = string.Join(OPERATIONS_SEPARATOR, operations);

            return $"{scope}: {operationsText}";
        }

        private static string DescribeScope(LevelBulkEditSettings settings, int stagesCount)
        {
            int firstStageNumber = settings.FirstStageIndex + 1;
            int lastStageNumber = settings.LastStageIndex + 1;

            if (firstStageNumber == FIRST_STAGE_NUMBER && lastStageNumber == stagesCount)
                return "весь уровень";

            if (firstStageNumber == lastStageNumber)
                return $"стейдж {firstStageNumber}";

            return $"стейджи {firstStageNumber}–{lastStageNumber}";
        }

        private static string FormatShift(float seconds)
        {
            string sign = seconds > 0f ? "+" : string.Empty;
            string value = LevelEditorTexts.Seconds(seconds);

            return $"{sign}{value}";
        }

        private static bool HasOperations(LevelBulkEditSettings settings)
            => settings.CountPercent != LevelBulkEditSettings.UNCHANGED_PERCENT
                || settings.DelayPercent != LevelBulkEditSettings.UNCHANGED_PERCENT
                || settings.StartShift != 0f
                || settings.StageTimeShift != 0f
                || settings.SkipShift != 0f;

        private void CreateGUI()
        {
            if (_config == null)
                return;

            StyleSheet styleSheet = LevelEditorWindowFiles.LoadNextToScript<StyleSheet>(this, STYLE_FILE_NAME);

            if (styleSheet != null)
                rootVisualElement.styleSheets.Add(styleSheet);

            rootVisualElement.AddToClassList(DialogElements.ROOT_CLASS);

            Label header = new($"Уровень {_config.name}: изменить сразу несколько стейджей");
            header.AddToClassList(DialogElements.HEADER_CLASS);

            _countField = CreateIntegerField(
                "Количество врагов, %",
                LevelBulkEditSettings.UNCHANGED_PERCENT,
                "В волне с одной группой меняется количество, с несколькими — повторы шаблона, чтобы сохранить "
                + "пропорцию смеси. Округление до целого, не меньше 1. 100 % — не менять");
            _delayField = CreateIntegerField(
                "Интервал спавна, %",
                LevelBulkEditSettings.UNCHANGED_PERCENT,
                "Интервал между выходами врагов волны: меньше — плотнее. 100 % — не менять");
            _startShiftField = CreateFloatField(
                "Сдвиг старта волн, с",
                "Старт всех волн стейджа: плюс — позже, минус — раньше, но не раньше начала стейджа. 0 — не менять");
            _stageTimeShiftField = CreateFloatField(
                "Время стейджа, ± с",
                "Сколько длится стейдж до начала следующего. 0 — не менять");
            _skipShiftField = CreateFloatField(
                "Скип, ± с",
                "Через сколько секунд от начала стейджа игроку предложат скип. 0 — не менять");

            VisualElement fields = new();
            fields.AddToClassList(FIELDS_CLASS);
            fields.Add(_countField);
            fields.Add(_delayField);
            fields.Add(_startShiftField);
            fields.Add(_stageTimeShiftField);
            fields.Add(_skipShiftField);

            _content = new ScrollView();
            _content.AddToClassList(DialogElements.CONTENT_CLASS);

            _footer = new Label();
            _footer.AddToClassList(DialogElements.FOOTER_CLASS);

            _applyButton = DialogElements.CreateButton("Применить", OnApplyClicked);

            VisualElement buttons = new();
            buttons.AddToClassList(DialogElements.BUTTONS_CLASS);
            buttons.Add(DialogElements.CreateButton("Отмена", Close));
            buttons.Add(_applyButton);

            rootVisualElement.Add(header);
            rootVisualElement.Add(CreateRangeRow());
            rootVisualElement.Add(fields);
            rootVisualElement.Add(_content);
            rootVisualElement.Add(_footer);
            rootVisualElement.Add(buttons);

            RefreshPreview();
        }

        // «Стейджи с [N] по [M]» и кнопки, которые ставят выбранный стейдж или весь уровень.
        private VisualElement CreateRangeRow()
        {
            int stagesCount = _config.Stages.Count;
            int selectedStageNumber = Mathf.Clamp(_selectedStageIndex + 1, FIRST_STAGE_NUMBER, stagesCount);

            _firstStageField = CreateStageField(selectedStageNumber);
            _lastStageField = CreateStageField(selectedStageNumber);

            Button selectedStageButton = DialogElements.CreateButton(
                $"Выбранный стейдж ({selectedStageNumber})",
                () => SetRange(selectedStageNumber, selectedStageNumber));
            Button allStagesButton = DialogElements.CreateButton("Весь уровень", () => SetRange(FIRST_STAGE_NUMBER, stagesCount));

            VisualElement row = new();
            row.AddToClassList(RANGE_CLASS);
            row.Add(CreateRangeLabel("Стейджи с"));
            row.Add(_firstStageField);
            row.Add(CreateRangeLabel("по"));
            row.Add(_lastStageField);
            row.Add(selectedStageButton);
            row.Add(allStagesButton);

            return row;
        }

        private Label CreateRangeLabel(string text)
        {
            Label label = new(text);
            label.AddToClassList(RANGE_LABEL_CLASS);

            return label;
        }

        private IntegerField CreateStageField(int stageNumber)
        {
            IntegerField field = new()
            {
                value = stageNumber,
            };

            field.AddToClassList(STAGE_FIELD_CLASS);
            field.RegisterValueChangedCallback(_ => RefreshPreview());

            return field;
        }

        private IntegerField CreateIntegerField(string label, int value, string tooltip)
        {
            IntegerField field = new(label)
            {
                value = value,
                tooltip = tooltip,
            };

            field.AddToClassList(FIELD_CLASS);
            field.RegisterValueChangedCallback(_ => RefreshPreview());

            return field;
        }

        private FloatField CreateFloatField(string label, string tooltip)
        {
            FloatField field = new(label)
            {
                tooltip = tooltip,
            };

            field.AddToClassList(FIELD_CLASS);
            field.RegisterValueChangedCallback(_ => RefreshPreview());

            return field;
        }

        private void SetRange(int firstStageNumber, int lastStageNumber)
        {
            _firstStageField.SetValueWithoutNotify(firstStageNumber);
            _lastStageField.SetValueWithoutNotify(lastStageNumber);

            RefreshPreview();
        }

        // Номера стейджей приводятся к уровню и упорядочиваются; проценты — не меньше 1.
        private LevelBulkEditSettings ReadSettings()
        {
            int stagesCount = _config.Stages.Count;
            int firstStageIndex = Mathf.Clamp(_firstStageField.value, FIRST_STAGE_NUMBER, stagesCount) - 1;
            int lastStageIndex = Mathf.Clamp(_lastStageField.value, FIRST_STAGE_NUMBER, stagesCount) - 1;

            return new LevelBulkEditSettings
            {
                FirstStageIndex = Mathf.Min(firstStageIndex, lastStageIndex),
                LastStageIndex = Mathf.Max(firstStageIndex, lastStageIndex),
                CountPercent = Mathf.Max(MIN_PERCENT, _countField.value),
                DelayPercent = Mathf.Max(MIN_PERCENT, _delayField.value),
                StartShift = _startShiftField.value,
                StageTimeShift = _stageTimeShiftField.value,
                SkipShift = _skipShiftField.value,
            };
        }

        private void RefreshPreview()
        {
            _settings = ReadSettings();
            _editedSlice = LevelSliceSerializer.CreateDataSlice(_config);

            List<string> limitNotes = LevelBulkEdit.Apply(_editedSlice, _settings);
            LevelSliceImportResult preview = LevelSliceImporter.Preview(_config, _editedSlice, _enemies);

            _content.Clear();

            DialogElements.AddSection(_content, "Изменения", preview.Changes);
            DialogElements.AddSection(_content, "Анализ после правки", preview.AnalysisChanges);
            DialogElements.AddSection(_content, "Упёрлись в границы", limitNotes, DialogElements.WARNING_LINE_CLASS);
            DialogElements.AddSection(_content, "Ошибки", preview.Errors, DialogElements.ERROR_LINE_CLASS);
            DialogElements.AddSection(_content, "Предупреждения после правки", preview.Warnings, DialogElements.WARNING_LINE_CLASS);

            _footer.text = DescribeOutcome(preview);
            _applyButton.SetEnabled(preview.HasErrors == false && preview.HasChanges);
        }

        private string DescribeOutcome(LevelSliceImportResult preview)
        {
            if (HasOperations(_settings) == false)
                return "Задайте операцию: проценты, отличные от 100, или сдвиг в секундах.";

            if (preview.HasErrors)
                return "Применить нельзя: уровень с такими ошибками пересобрать нельзя — исправьте их в окне редактора.";

            if (preview.HasChanges == false)
                return "С такими значениями в выбранных стейджах ничего не меняется.";

            return "Применение — один шаг Undo, уровень останется несохранённым.";
        }

        private void OnApplyClicked()
        {
            _applied?.Invoke(_editedSlice, _settings);
            Close();
        }
    }
}
