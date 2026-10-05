using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Таблица вкладки «Все уровни»: строка на уровень, только чтение. Выбор строки выделяет уровень на кривой
    // сложности, щелчок по пустому месту под строками снимает выбор, двойной щелчок или Enter открывает уровень
    // на вкладке «Уровень».
    public class LevelsTable
    {
        private const string LABEL_CLASS = "table-label";
        private const string WARNING_LABEL_CLASS = "table-label--warning";
        private const string ERROR_LABEL_CLASS = "table-label--error";
        private const string MISSING_VALUE = "—";
        private const string DIRTY_MARK = " ●";
        private const string NO_ISSUES = "нет";
        private const string ISSUES_SEPARATOR = "  ";
        private const string LINE_BREAK = "\n";
        private const string OPEN_HINT = "Двойной щелчок — открыть уровень";

        // Подсказка ячейки замечаний перечисляет не больше стольких замечаний.
        private const int MAX_TOOLTIP_ISSUES = 10;

        public event Action<LevelConfig> LevelSelected;
        public event Action<LevelConfig> LevelChosen;

        private readonly MultiColumnListView _table;
        private readonly ScrollView _scrollView;
        private readonly Label _title;
        private readonly List<LevelOverview> _levels = new();

        public LevelsTable(VisualElement root)
        {
            _table = root.Q<MultiColumnListView>("levelsTable");
            _scrollView = _table.Q<ScrollView>();
            _title = root.Q<Label>("levelsTitle");

            _table.itemsSource = _levels;
            _table.selectedIndicesChanged += OnSelectedIndicesChanged;
            _table.itemsChosen += OnItemsChosen;
            _scrollView.RegisterCallback<PointerDownEvent>(OnPointerDown);

            SetupColumn("number", BindNumber);
            SetupColumn("level", BindLevel);
            SetupColumn("stages", (label, level) => label.text = level.Stats.StagesCount.ToString());
            SetupColumn("waves", (label, level) => label.text = level.Stats.WavesCount.ToString());
            SetupColumn("enemies", (label, level) => label.text = level.Stats.EnemiesCount.ToString());
            SetupColumn("health", (label, level) => label.text = LevelEditorTexts.Number(level.Stats.Health));
            SetupColumn("effectiveHealth", BindEffectiveHealth);
            SetupColumn("gold", BindGold);
            SetupColumn("duration", BindDuration);
            SetupColumn("threat", BindThreat);
            SetupColumn("peak", BindPeak);
            SetupColumn("issues", BindIssues);
        }

        public void Refresh(IReadOnlyList<LevelOverview> levels, LevelConfig selectedLevel)
        {
            bool isCountChanged = levels.Count != _levels.Count;

            _levels.Clear();
            _levels.AddRange(levels);

            if (isCountChanged)
                _table.Rebuild();
            else
                _table.RefreshItems();

            int selectedIndex = _levels.FindIndex(level => level.Config == selectedLevel);
            CollectionViewIndices.Select(_table, selectedIndex >= 0 ? selectedIndex : LevelAddress.NO_INDEX);

            int levelsCount = _levels.Count;
            _title.text = $"Уровни: {levelsCount}";
        }

        private static string DescribeIssues(IReadOnlyList<LevelIssue> issues)
        {
            List<string> lines = new()
            {
                LevelEditorTexts.DescribeIssuesCount(issues),
            };

            lines.AddRange(issues.Take(MAX_TOOLTIP_ISSUES).Select(LevelEditorTexts.FormatIssue));

            int hiddenCount = issues.Count - MAX_TOOLTIP_ISSUES;

            if (hiddenCount > 0)
                lines.Add($"…и ещё {hiddenCount}");

            return string.Join(LINE_BREAK, lines);
        }

        private void SetupColumn(string columnName, Action<Label, LevelOverview> bind)
        {
            Column column = _table.columns[columnName];

            column.makeCell = () =>
            {
                Label label = new();
                label.AddToClassList(LABEL_CLASS);

                return label;
            };

            column.bindCell = (element, index) => bind((Label)element, _levels[index]);
        }

        private void BindNumber(Label label, LevelOverview level)
        {
            bool isInCampaign = level.CampaignNumber.HasValue;

            label.text = isInCampaign ? level.CampaignNumber.Value.ToString() : MISSING_VALUE;
            label.tooltip = isInCampaign ? "Номер уровня в кампании" : "Уровень вне кампании";
        }

        private void BindLevel(Label label, LevelOverview level)
        {
            string levelName = level.Config.name;

            label.text = level.IsDirty ? $"{levelName}{DIRTY_MARK}" : levelName;
            label.tooltip = level.IsDirty ? $"Есть несохранённые правки{LINE_BREAK}{OPEN_HINT}" : OPEN_HINT;
        }

        private void BindEffectiveHealth(Label label, LevelOverview level)
        {
            string physic = LevelEditorTexts.Number(level.Stats.EffectiveHealthPhysic);
            string magic = LevelEditorTexts.Number(level.Stats.EffectiveHealthMagic);

            label.text = $"{physic} / {magic}";
            label.tooltip = "Сколько физического / магического урона нужно нанести с учётом сопротивлений";
        }

        private void BindGold(Label label, LevelOverview level)
        {
            int startGold = level.Config.StartGold;
            int totalGold = level.Stats.TotalGoldWithoutSkips;
            int totalGoldWithSkips = level.Stats.TotalGoldWithMaxSkips;

            label.text = $"{startGold} → {totalGold} ({totalGoldWithSkips})";
            label.tooltip = $"Стартовое золото {startGold}, к концу уровня {totalGold} без скипов "
                + $"и {totalGoldWithSkips} с максимумом скипов — будто все враги убиты";
        }

        private void BindDuration(Label label, LevelOverview level)
        {
            string duration = LevelEditorTexts.Seconds(level.Stats.DurationWithoutSkips);
            string durationWithSkips = LevelEditorTexts.Seconds(level.Stats.DurationWithMaxSkips);

            label.text = $"{duration} ({durationWithSkips})";
            label.tooltip = $"Последний враг уровня выходит на {duration} без скипов "
                + $"и на {durationWithSkips}, если скипать сразу, как разрешено";
        }

        private void BindThreat(Label label, LevelOverview level)
        {
            string threat = LevelEditorTexts.Number(level.Stats.BreakthroughThreat);
            int damage = level.Stats.BreakthroughDamage;
            int playerHealth = level.Config.StartPlayerHealth;

            label.text = $"×{threat}";
            label.tooltip = "Урон по базе, если прорвутся все враги уровня, в долях здоровья игрока: "
                + $"урон {damage} при здоровье {playerHealth}";
        }

        private void BindPeak(Label label, LevelOverview level)
        {
            int? peak = level.Stats.PeakEnemies;

            label.text = peak.HasValue ? peak.Value.ToString() : MISSING_VALUE;
            label.tooltip = "Пик врагов на поле — оценка без убийств";
        }

        // Число ошибок и предупреждений; в подсказке — сами замечания.
        private void BindIssues(Label label, LevelOverview level)
        {
            int errorsCount = level.Issues.Count(issue => issue.IsError);
            int warningsCount = level.Issues.Count - errorsCount;
            List<string> counters = new();

            if (errorsCount > 0)
                counters.Add($"{LevelEditorTexts.ERROR_MARKER}{errorsCount}");

            if (warningsCount > 0)
                counters.Add($"{LevelEditorTexts.WARNING_MARKER}{warningsCount}");

            label.text = counters.Count > 0 ? string.Join(ISSUES_SEPARATOR, counters) : NO_ISSUES;
            label.tooltip = DescribeIssues(level.Issues);
            label.EnableInClassList(ERROR_LABEL_CLASS, errorsCount > 0);
            label.EnableInClassList(WARNING_LABEL_CLASS, errorsCount == 0 && warningsCount > 0);
        }

        // Список с одиночным выбором щелчки мимо строк пропускает — выбор снимаем сами. Пустое место определяется
        // по координатам: внутри видимой области таблицы (полосы прокрутки в неё не входят) и ниже последней строки.
        // Какой внутренний элемент прокрутки получит щелчок, зависит от устройства ScrollView, поэтому цель не сверяем.
        private void OnPointerDown(PointerDownEvent pointerEvent)
        {
            if (pointerEvent.button != (int)MouseButton.LeftMouse)
                return;

            Vector2 position = pointerEvent.position;
            bool isInViewport = _scrollView.contentViewport.worldBound.Contains(position);
            bool isBelowRows = position.y > _scrollView.contentContainer.worldBound.yMax;

            if (isInViewport && isBelowRows)
                _table.ClearSelection();
        }

        private void OnSelectedIndicesChanged(IEnumerable<int> indices)
        {
            int index = indices.DefaultIfEmpty(LevelAddress.NO_INDEX).First();
            LevelConfig level = index != LevelAddress.NO_INDEX ? _levels[index].Config : null;

            LevelSelected?.Invoke(level);
        }

        // Окно уходит на другую вкладку — на следующем кадре, а не внутри события таблицы.
        private void OnItemsChosen(IEnumerable<object> items)
        {
            LevelOverview level = items.OfType<LevelOverview>().FirstOrDefault();

            if (level != null)
                _table.schedule.Execute(() => LevelChosen?.Invoke(level.Config));
        }
    }
}
