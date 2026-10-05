using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Вкладка «Все уровни»: таблица итогов по всем LevelConfig и кривая сложности кампании под ней.
    // Считается по конфигам в памяти, поэтому несохранённые правки видны сразу. Выбранный в таблице уровень
    // выделяется на графике; двойной щелчок по строке и щелчок по столбику открывают уровень на вкладке «Уровень».
    public class AllLevelsPage
    {
        private const string MEASURE_BUTTON_CLASS = "segmented-buttons__button";
        private const string SELECTED_MEASURE_CLASS = "segmented-buttons__button--selected";

        private static readonly DifficultyMeasure[] _measures = (DifficultyMeasure[])Enum.GetValues(typeof(DifficultyMeasure));

        public event Action<LevelConfig> LevelChosen;
        public event Action<LevelConfig, int> StageChosen;
        public event Action<DifficultyMeasure> MeasureChanged;

        private readonly LevelsTable _table;
        private readonly DifficultyChart _chart;
        private readonly Label _chartTitle;
        private readonly List<Button> _measureButtons = new();
        private readonly List<LevelOverview> _levels = new();

        private DifficultyMeasure _measure;
        private LevelConfig _selectedLevel;

        public AllLevelsPage(VisualElement root, DifficultyMeasure measure)
        {
            _measure = measure;
            _table = new LevelsTable(root);
            _chart = new DifficultyChart();
            _chartTitle = root.Q<Label>("difficultyTitle");

            root.Q<VisualElement>("difficultyChartSlot").Add(_chart);
            CreateMeasureButtons(root.Q<VisualElement>("difficultyMeasures"));

            _table.LevelSelected += OnLevelSelected;
            _table.LevelChosen += level => LevelChosen?.Invoke(level);
            _chart.StageClicked += (level, stageIndex) => StageChosen?.Invoke(level, stageIndex);

            RefreshMeasureButtons();
        }

        public void Refresh(IReadOnlyList<LevelConfig> levels, LevelsListConfig campaign)
        {
            GameConfig gameConfig = LevelEditorAssets.FindGameConfig();
            IEnumerable<LevelOverview> overviews = levels
                .Where(level => level != null)
                .Select(level => CreateOverview(level, campaign, gameConfig));

            _levels.Clear();
            _levels.AddRange(overviews);

            _table.Refresh(_levels, _selectedLevel);
            RefreshChart();
        }

        // Уровень, который изменили или сохранили со стороны — в инспекторе, через Ctrl+S, импортом среза, — пересчитывается.
        public void RefreshIfChangedOutside(IReadOnlyList<LevelConfig> levels, LevelsListConfig campaign)
        {
            if (_levels.Any(IsChangedOutside))
                Refresh(levels, campaign);
        }

        private static LevelOverview CreateOverview(LevelConfig level, LevelsListConfig campaign, GameConfig gameConfig)
        {
            LevelPrefabInfo prefab = LevelPrefabReader.Read(level);

            return new LevelOverview
            {
                Config = level,
                CampaignNumber = LevelEditorAssets.GetCampaignNumber(campaign, level),
                Stats = LevelStatsCalculator.Calculate(level, prefab, gameConfig),
                Issues = LevelValidator.Validate(level, prefab),
                IsDirty = EditorUtility.IsDirty(level),
                DirtyCount = EditorUtility.GetDirtyCount(level),
            };
        }

        // Удалённый уровень здесь не считается: список уровней перечитает окно по изменению проекта.
        private static bool IsChangedOutside(LevelOverview level)
        {
            if (level.Config == null)
                return false;

            return EditorUtility.GetDirtyCount(level.Config) != level.DirtyCount || EditorUtility.IsDirty(level.Config) != level.IsDirty;
        }

        private void CreateMeasureButtons(VisualElement container)
        {
            foreach (DifficultyMeasure measure in _measures)
            {
                Button button = new(() => SelectMeasure(measure))
                {
                    text = LevelEditorTexts.GetMeasureName(measure),
                    tooltip = LevelEditorTexts.DescribeMeasure(measure),
                };

                button.AddToClassList(MEASURE_BUTTON_CLASS);
                container.Add(button);
                _measureButtons.Add(button);
            }
        }

        private void SelectMeasure(DifficultyMeasure measure)
        {
            if (measure == _measure)
                return;

            _measure = measure;

            MeasureChanged?.Invoke(measure);
            RefreshMeasureButtons();
            RefreshChart();
        }

        private void RefreshMeasureButtons()
        {
            for (int i = 0; i < _measures.Length; i++)
                _measureButtons[i].EnableInClassList(SELECTED_MEASURE_CLASS, _measures[i] == _measure);

            string measureName = LevelEditorTexts.GetMeasureName(_measure);
            _chartTitle.text = $"Кривая сложности · {measureName} по стейджам кампании";
        }

        // На графике — только уровни кампании со стейджами, по порядку кампании.
        private void RefreshChart()
        {
            List<LevelOverview> campaignLevels = _levels
                .Where(level => level.CampaignNumber.HasValue && level.Stats.Stages.Count > 0)
                .OrderBy(level => level.CampaignNumber.Value)
                .ToList();

            _chart.SetData(campaignLevels, _measure, _selectedLevel);
        }

        private void OnLevelSelected(LevelConfig level)
        {
            _selectedLevel = level;
            _chart.SetSelectedLevel(level);
        }
    }
}
