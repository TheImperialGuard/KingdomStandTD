using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Панель таймлайна над таблицей волн: переключатель «Стейдж / Уровень», таймлайн и легенда врагов.
    // Таймлайн только показывает и выбирает, правка — в таблице и деталях волны.
    public class TimelinePanel
    {
        private const string SELECTED_MODE_CLASS = "timeline-modes__button--selected";
        private const string LEGEND_ITEM_CLASS = "timeline-legend__item";
        private const string LEGEND_SWATCH_CLASS = "timeline-legend__swatch";

        public event Action<int, int> WaveChosen;
        public event Action<int> StageChosen;
        public event Action<TimelineMode> ModeChanged;

        private readonly LevelTimeline _timeline;
        private readonly Label _title;
        private readonly Button _stageModeButton;
        private readonly Button _levelModeButton;
        private readonly VisualElement _legend;

        private LevelEditingSession _session;
        private TimelineMode _mode;
        private int _stageIndex = LevelAddress.NO_INDEX;
        private int _waveIndex = LevelAddress.NO_INDEX;

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

            RefreshModeButtons();
        }

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex, int waveIndex)
        {
            _stageIndex = stageIndex;
            _waveIndex = waveIndex;

            if (_session == null || _session.IsAlive == false)
                return;

            TimelineModel model = TimelineModelBuilder.Build(_session.Config, _session.Stats, _session.Prefab, _mode, stageIndex);

            _timeline.SetModel(model, stageIndex, waveIndex);
            _title.text = DescribeTitle(stageIndex);

            RefreshLegend(model.Enemies);
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
        private void RefreshLegend(IReadOnlyList<TimelineEnemy> enemies)
        {
            _legend.Clear();

            foreach (TimelineEnemy enemy in enemies)
            {
                VisualElement swatch = new();
                swatch.AddToClassList(LEGEND_SWATCH_CLASS);
                swatch.style.backgroundColor = TimelinePalette.GetEnemyColor(enemy.Slot);

                VisualElement item = new()
                {
                    tooltip = enemy.Tooltip,
                };

                item.AddToClassList(LEGEND_ITEM_CLASS);
                item.Add(swatch);
                item.Add(new Label(enemy.Name));

                _legend.Add(item);
            }
        }
    }
}
