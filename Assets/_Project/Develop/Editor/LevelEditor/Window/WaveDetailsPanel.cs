using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Детали выбранной волны: шаблон групп врагов, число его повторов и заметка.
    public class WaveDetailsPanel
    {
        private const string HIDDEN_CONTENT_CLASS = "details__content--hidden";

        private readonly EnemyCatalog _enemyCatalog;
        private readonly Label _title;
        private readonly VisualElement _content;
        private readonly VisualElement _groupsContainer;
        private readonly IntegerField _repeatsField;
        private readonly Label _totalLabel;
        private readonly TextField _noteField;
        private readonly List<WaveGroupRow> _groupRows = new();

        private LevelEditingSession _session;
        private int _stageIndex = LevelAddress.NO_INDEX;
        private int _waveIndex = LevelAddress.NO_INDEX;

        public WaveDetailsPanel(VisualElement root, EnemyCatalog enemyCatalog)
        {
            _enemyCatalog = enemyCatalog;

            _title = root.Q<Label>("waveTitle");
            _content = root.Q<VisualElement>("waveDetailsContent");
            _groupsContainer = root.Q<VisualElement>("groupsContainer");
            _repeatsField = root.Q<IntegerField>("repeatsField");
            _totalLabel = root.Q<Label>("waveTotalLabel");
            _noteField = root.Q<TextField>("waveNoteField");

            root.Q<Button>("addGroupButton").clicked += OnAddGroupClicked;
            _repeatsField.RegisterValueChangedCallback(changeEvent => OnRepeatsChanged(changeEvent.newValue));
            _noteField.RegisterValueChangedCallback(changeEvent => OnNoteChanged(changeEvent.newValue));
        }

        private bool IsWaveSelected
            => _session != null && _stageIndex != LevelAddress.NO_INDEX && _waveIndex != LevelAddress.NO_INDEX;

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex, int waveIndex)
        {
            _stageIndex = stageIndex;
            _waveIndex = waveIndex;

            _content.EnableInClassList(HIDDEN_CONTENT_CLASS, IsWaveSelected == false);

            if (IsWaveSelected == false)
            {
                _title.text = "Волна не выбрана";
                return;
            }

            EnemiesWaveConfig wave = LevelStatsCalculator.GetWaves(_session.Config.Stages[stageIndex])[waveIndex];
            WaveStats stats = _session.Stats.Stages[stageIndex].Waves[waveIndex];
            IReadOnlyList<WaveEnemyGroup> groups = LevelStatsCalculator.GetGroups(wave);
            int patternCount = groups.Sum(group => group.Count);

            _title.text = LevelAddress.Format(stageIndex, waveIndex);
            _repeatsField.SetValueWithoutNotify(wave.PatternRepeats);
            _totalLabel.text = $"Всего врагов {stats.EnemiesCount}: шаблон из {patternCount} × повторов {wave.PatternRepeats}";
            _noteField.SetValueWithoutNotify(wave.Note ?? string.Empty);

            RefreshGroups(groups);
        }

        // Строки групп переиспользуются: пересоздаются только лишние или недостающие.
        private void RefreshGroups(IReadOnlyList<WaveEnemyGroup> groups)
        {
            while (_groupRows.Count < groups.Count)
            {
                WaveGroupRow row = CreateGroupRow();

                _groupRows.Add(row);
                _groupsContainer.Add(row);
            }

            while (_groupRows.Count > groups.Count)
            {
                int lastIndex = _groupRows.Count - 1;

                _groupRows[lastIndex].RemoveFromHierarchy();
                _groupRows.RemoveAt(lastIndex);
            }

            for (int i = 0; i < groups.Count; i++)
                _groupRows[i].Bind(i, groups[i], groups.Count);
        }

        private WaveGroupRow CreateGroupRow()
        {
            WaveGroupRow row = new(_enemyCatalog);

            row.EnemyChanged += (groupIndex, enemy) => _session.SetGroupEnemy(_stageIndex, _waveIndex, groupIndex, enemy);
            row.CountChanged += (groupIndex, count) => _session.SetGroupCount(_stageIndex, _waveIndex, groupIndex, count);
            row.MoveUpClicked += groupIndex => _session.MoveGroup(_stageIndex, _waveIndex, groupIndex, groupIndex - 1);
            row.MoveDownClicked += groupIndex => _session.MoveGroup(_stageIndex, _waveIndex, groupIndex, groupIndex + 1);
            row.RemoveClicked += groupIndex => _session.RemoveGroup(_stageIndex, _waveIndex, groupIndex);

            return row;
        }

        private void OnAddGroupClicked()
        {
            if (IsWaveSelected)
                _session.AddGroup(_stageIndex, _waveIndex);
        }

        private void OnRepeatsChanged(int value)
        {
            if (IsWaveSelected)
                _session.SetWavePatternRepeats(_stageIndex, _waveIndex, value);
        }

        private void OnNoteChanged(string value)
        {
            if (IsWaveSelected)
                _session.SetWaveNote(_stageIndex, _waveIndex, value);
        }
    }
}
