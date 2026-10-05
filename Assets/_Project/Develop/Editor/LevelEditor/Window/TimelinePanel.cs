using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Панель таймлайна над таблицей волн: переключатель «Стейдж / Уровень», таймлайн и легенда врагов.
    // Таймлайн только показывает и выбирает, правка — в таблице и деталях волны. Наведение на врага в легенде
    // подсвечивает его на таймлайне, щелчок закрепляет подсветку. Закреплённый враг запоминается по конфигу,
    // поэтому подсветка переживает правки и переходит в другие уровни, где этот враг есть.
    public class TimelinePanel
    {
        private const string SELECTED_MODE_CLASS = "segmented-buttons__button--selected";
        private const string LEGEND_ITEM_CLASS = "timeline-legend__item";
        private const string LEGEND_ITEM_PINNED_CLASS = "timeline-legend__item--pinned";
        private const string LEGEND_ITEM_DIMMED_CLASS = "timeline-legend__item--dimmed";
        private const string LEGEND_SWATCH_CLASS = "timeline-legend__swatch";
        private const string PARAGRAPH_BREAK = "\n\n";
        private const string PIN_HINT = "Щелчок — закрепить подсветку врага на таймлайне";
        private const string UNPIN_HINT = "Щелчок — снять подсветку";

        public event Action<int, int> WaveChosen;
        public event Action<int> StageChosen;
        public event Action<TimelineMode> ModeChanged;

        private readonly LevelTimeline _timeline;
        private readonly Label _title;
        private readonly Button _stageModeButton;
        private readonly Button _levelModeButton;
        private readonly VisualElement _legend;
        private readonly List<VisualElement> _legendItems = new();

        private LevelEditingSession _session;
        private TimelineMode _mode;
        private TimelineModel _model = new();
        private int _stageIndex = LevelAddress.NO_INDEX;
        private int _waveIndex = LevelAddress.NO_INDEX;
        private CharacterConfig _hoveredEnemy;
        private CharacterConfig _pinnedEnemy;

        public TimelinePanel(VisualElement root, TimelineMode mode)
        {
            _mode = mode;
            _timeline = new LevelTimeline();
            _title = root.Q<Label>("timelineTitle");
            _stageModeButton = root.Q<Button>("timelineStageModeButton");
            _levelModeButton = root.Q<Button>("timelineLevelModeButton");
            _legend = root.Q<VisualElement>("timelineLegend");

            root.Q<ScrollView>("timelineScroll").Add(_timeline);

            _stageModeButton.clicked += () => SelectMode(TimelineMode.Stage);
            _levelModeButton.clicked += () => SelectMode(TimelineMode.Level);
            _timeline.WaveClicked += (stageIndex, waveIndex) => WaveChosen?.Invoke(stageIndex, waveIndex);
            _timeline.StageClicked += stageIndex => StageChosen?.Invoke(stageIndex);

            // Строки легенды пересоздаются при каждом обновлении, а сама легенда остаётся: уход курсора с неё
            // снимает подсветку по наведению, даже если строка под курсором успела смениться.
            _legend.RegisterCallback<PointerLeaveEvent>(_ => SetHoveredEnemy(null));

            RefreshModeButtons();
        }

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex, int waveIndex)
        {
            _stageIndex = stageIndex;
            _waveIndex = waveIndex;

            if (_session == null || _session.IsAlive == false)
                return;

            _model = TimelineModelBuilder.Build(_session.Config, _session.Stats, _session.Prefab, _mode, stageIndex);

            _timeline.SetModel(_model, stageIndex, waveIndex);
            _title.text = DescribeTitle(stageIndex);

            RebuildLegend();
            RefreshHighlight();
        }

        private void SelectMode(TimelineMode mode)
        {
            if (mode == _mode)
                return;

            _mode = mode;

            RefreshModeButtons();
            ModeChanged?.Invoke(mode);
            Refresh(_stageIndex, _waveIndex);
        }

        private string DescribeTitle(int stageIndex)
        {
            if (_mode == TimelineMode.Level)
                return "Таймлайн · весь уровень, стейджи подряд без скипов";

            string stageAddress = stageIndex != LevelAddress.NO_INDEX ? LevelAddress.Format(stageIndex) : "стейдж не выбран";

            return $"Таймлайн · {stageAddress}";
        }

        private void RefreshModeButtons()
        {
            _stageModeButton.EnableInClassList(SELECTED_MODE_CLASS, _mode == TimelineMode.Stage);
            _levelModeButton.EnableInClassList(SELECTED_MODE_CLASS, _mode == TimelineMode.Level);
        }

        // Легенда — враги уровня с их цветами: имя дублирует цвет, в подсказке — статы врага.
        // Подсказки и отметки подсветки выставляет RefreshHighlight.
        private void RebuildLegend()
        {
            _legend.Clear();
            _legendItems.Clear();

            foreach (TimelineEnemy enemy in _model.Enemies)
            {
                CharacterConfig enemyConfig = enemy.Config;

                VisualElement swatch = new();
                swatch.AddToClassList(LEGEND_SWATCH_CLASS);
                swatch.style.backgroundColor = TimelinePalette.GetEnemyColor(enemy.Slot);

                VisualElement item = new();
                item.AddToClassList(LEGEND_ITEM_CLASS);
                item.Add(swatch);
                item.Add(new Label(enemy.Name));

                item.RegisterCallback<PointerEnterEvent>(_ => SetHoveredEnemy(enemyConfig));
                item.RegisterCallback<PointerLeaveEvent>(_ => OnLegendItemLeft(enemyConfig));
                item.RegisterCallback<ClickEvent>(_ => TogglePinnedEnemy(enemyConfig));

                _legend.Add(item);
                _legendItems.Add(item);
            }
        }

        // Подсвечен враг под курсором, а если курсор не на легенде — закреплённый. Остальные строки легенды гаснут.
        private void RefreshHighlight()
        {
            CharacterConfig highlightedEnemy = _hoveredEnemy != null ? _hoveredEnemy : _pinnedEnemy;
            int highlightedIndex = FindEnemyIndex(highlightedEnemy);
            bool isHighlighting = highlightedIndex != LevelAddress.NO_INDEX;

            _timeline.SetHighlightedEnemy(highlightedIndex);

            for (int i = 0; i < _legendItems.Count; i++)
            {
                TimelineEnemy enemy = _model.Enemies[i];
                VisualElement item = _legendItems[i];
                bool isPinned = enemy.Config == _pinnedEnemy;
                string hint = isPinned ? UNPIN_HINT : PIN_HINT;

                item.EnableInClassList(LEGEND_ITEM_PINNED_CLASS, isPinned);
                item.EnableInClassList(LEGEND_ITEM_DIMMED_CLASS, isHighlighting && i != highlightedIndex);
                item.tooltip = $"{enemy.Tooltip}{PARAGRAPH_BREAK}{hint}";
            }
        }

        private int FindEnemyIndex(CharacterConfig enemy)
        {
            if (enemy == null)
                return LevelAddress.NO_INDEX;

            int index = _model.Enemies.FindIndex(timelineEnemy => timelineEnemy.Config == enemy);

            return index >= 0 ? index : LevelAddress.NO_INDEX;
        }

        private void SetHoveredEnemy(CharacterConfig enemy)
        {
            _hoveredEnemy = enemy;
            RefreshHighlight();
        }

        // Курсор перешёл на соседнюю строку раньше, чем ушёл с этой, — подсветка остаётся за соседней.
        private void OnLegendItemLeft(CharacterConfig enemy)
        {
            if (_hoveredEnemy == enemy)
                SetHoveredEnemy(null);
        }

        private void TogglePinnedEnemy(CharacterConfig enemy)
        {
            _pinnedEnemy = _pinnedEnemy == enemy ? null : enemy;
            RefreshHighlight();
        }
    }
}
