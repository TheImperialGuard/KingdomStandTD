using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Таблица волн выбранного стейджа. Дорога, полоса, старт, интервал и повторы правятся прямо в ячейках,
    // состав шаблона — в деталях волны. Волна добавляется кнопкой и контекстным меню, переставляется
    // перетаскиванием и переносится в другой стейдж из меню; копирование, вставка, дублирование и удаление —
    // в меню, колонкой ✕ и командами Ctrl+C, Ctrl+V, Ctrl+D и Del.
    public class WavesTable
    {
        private const string LABEL_CLASS = "table-label";
        private const string WARNING_LABEL_CLASS = "table-label--warning";
        private const string FIELD_CLASS = "table-field";
        private const string REMOVE_BUTTON_CLASS = "table-remove-button";
        private const string ROAD_CELL_CLASS = "road-cell";
        private const string ROAD_DROPDOWN_CLASS = "road-cell__dropdown";
        private const string ROAD_SWATCH_CLASS = "road-swatch";
        private const string ROAD_SWATCH_CLASS_PREFIX = "road-swatch--";
        private const string MOVE_TO_STAGE_ITEM = "Перенести в стейдж";

        public event Action<int> WaveSelected;

        private readonly LevelEditorCommands _commands;
        private readonly MultiColumnListView _table;
        private readonly Label _title;
        private readonly Button _addWaveButton;
        private readonly List<int> _waveIndices = new();
        private readonly List<string> _roadChoices;
        private readonly PathNumber[] _pathNumbers = (PathNumber[])Enum.GetValues(typeof(PathNumber));
        private readonly string[] _swatchClasses;

        private LevelEditingSession _session;
        private int _stageIndex = LevelAddress.NO_INDEX;
        private int _waveIndex = LevelAddress.NO_INDEX;

        public WavesTable(VisualElement root, LevelEditorCommands commands)
        {
            _commands = commands;
            _table = root.Q<MultiColumnListView>("wavesTable");
            _title = root.Q<Label>("wavesTitle");
            _addWaveButton = root.Q<Button>("addWaveButton");
            _roadChoices = _pathNumbers.Select(pathNumber => pathNumber.ToString()).ToList();
            _swatchClasses = _pathNumbers.Select(GetSwatchClass).ToArray();

            _table.itemsSource = _waveIndices;
            _table.selectedIndicesChanged += OnSelectedIndicesChanged;
            _table.itemIndexChanged += OnWaveMoved;
            _table.Q<ScrollView>().AddManipulator(new ContextualMenuManipulator(BuildWaveMenu));

            CollectionViewCommands.Register(_table, CopySelectedWave, PasteAfterSelectedWave, DuplicateSelectedWave, RemoveSelectedWave);
            _addWaveButton.clicked += OnAddWaveClicked;

            SetupLabelColumn("number", BindNumber);
            SetupLabelColumn("composition", BindComposition);
            SetupRoadColumn();
            SetupLaneColumn();
            SetupFloatColumn("startDelay", wave => wave.StartDelay, (waveIndex, value) => _session.SetWaveStartDelay(_stageIndex, waveIndex, value));
            SetupFloatColumn("delay", wave => wave.DelayBetweenSpawns, (waveIndex, value) => _session.SetWaveDelayBetweenSpawns(_stageIndex, waveIndex, value));
            SetupIntegerColumn("repeats", wave => wave.PatternRepeats, (waveIndex, value) => _session.SetWavePatternRepeats(_stageIndex, waveIndex, value));
            SetupLabelColumn("total", (label, waveIndex) => label.text = GetWaveStats(waveIndex).EnemiesCount.ToString());
            SetupLabelColumn("spawnEnd", BindSpawnEnd);
            SetupLabelColumn("health", (label, waveIndex) => label.text = LevelEditorTexts.Number(GetWaveStats(waveIndex).Health));
            SetupLabelColumn("gold", (label, waveIndex) => label.text = GetWaveStats(waveIndex).KillGold.ToString());
            SetupRemoveColumn();
        }

        private bool IsStageSelected => _session != null && _stageIndex != LevelAddress.NO_INDEX;
        private bool IsWaveSelected => IsStageSelected && _waveIndex != LevelAddress.NO_INDEX;

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex, int waveIndex)
        {
            bool isSelectionChanged = stageIndex != _stageIndex || waveIndex != _waveIndex;
            _stageIndex = stageIndex;
            _waveIndex = waveIndex;

            UpdateRoadChoices();

            bool isStageSelected = stageIndex != LevelAddress.NO_INDEX;
            int wavesCount = isStageSelected ? _session.GetWavesCount(stageIndex) : 0;
            string stageAddress = isStageSelected ? LevelAddress.Format(stageIndex) : "стейдж не выбран";

            _title.text = $"Волны · {stageAddress}";
            _addWaveButton.SetEnabled(isStageSelected);

            CollectionViewIndices.Sync(_table, _waveIndices, wavesCount);
            CollectionViewIndices.Select(_table, waveIndex);

            if (isSelectionChanged && waveIndex != LevelAddress.NO_INDEX)
                _table.ScrollToItem(waveIndex);
        }

        private EnemiesWaveConfig GetWave(int waveIndex)
            => LevelStatsCalculator.GetWaves(_session.Config.Stages[_stageIndex])[waveIndex];

        private WaveStats GetWaveStats(int waveIndex) => _session.Stats.Stages[_stageIndex].Waves[waveIndex];

        private void UpdateRoadChoices()
        {
            for (int i = 0; i < _pathNumbers.Length; i++)
                _roadChoices[i] = LevelEditorTexts.DescribeRoadChoice(_pathNumbers[i], _session.Prefab);
        }

        private void SetupLabelColumn(string columnName, Action<Label, int> bind)
        {
            Column column = _table.columns[columnName];

            column.makeCell = () =>
            {
                Label label = new();
                label.AddToClassList(LABEL_CLASS);

                return label;
            };

            column.bindCell = (element, waveIndex) =>
            {
                element.userData = waveIndex;
                bind((Label)element, waveIndex);
            };
        }

        private void SetupRemoveColumn()
        {
            Column column = _table.columns["remove"];

            column.makeCell = () =>
            {
                Button button = new()
                {
                    text = "✕",
                    tooltip = "Удалить волну",
                };

                button.AddToClassList(REMOVE_BUTTON_CLASS);
                button.clicked += () => _commands.RemoveWave(_stageIndex, (int)button.userData);

                return button;
            };

            column.bindCell = (element, waveIndex) => element.userData = waveIndex;
        }

        private void SetupFloatColumn(string columnName, Func<EnemiesWaveConfig, float> read, Action<int, float> write)
        {
            Column column = _table.columns[columnName];

            column.makeCell = () =>
            {
                FloatField field = new()
                {
                    isDelayed = true,
                };

                field.AddToClassList(FIELD_CLASS);
                field.RegisterValueChangedCallback(changeEvent => write((int)field.userData, changeEvent.newValue));

                return field;
            };

            column.bindCell = (element, waveIndex) =>
            {
                FloatField field = (FloatField)element;
                field.userData = waveIndex;
                field.SetValueWithoutNotify(read(GetWave(waveIndex)));
            };
        }

        private void SetupIntegerColumn(string columnName, Func<EnemiesWaveConfig, int> read, Action<int, int> write)
        {
            Column column = _table.columns[columnName];

            column.makeCell = () =>
            {
                IntegerField field = new()
                {
                    isDelayed = true,
                };

                field.AddToClassList(FIELD_CLASS);
                field.RegisterValueChangedCallback(changeEvent => write((int)field.userData, changeEvent.newValue));

                return field;
            };

            column.bindCell = (element, waveIndex) =>
            {
                IntegerField field = (IntegerField)element;
                field.userData = waveIndex;
                field.SetValueWithoutNotify(read(GetWave(waveIndex)));
            };
        }

        private void SetupRoadColumn()
        {
            Column column = _table.columns["road"];

            column.makeCell = CreateRoadCell;
            column.bindCell = BindRoadCell;
        }

        private void SetupLaneColumn()
        {
            Column column = _table.columns["lane"];

            column.makeCell = () =>
            {
                LaneSelector selector = new();
                selector.ValueChanged += lane => _session.SetWaveLane(_stageIndex, (int)selector.userData, lane);

                return selector;
            };

            column.bindCell = (element, waveIndex) =>
            {
                LaneSelector selector = (LaneSelector)element;
                selector.userData = waveIndex;
                selector.SetValueWithoutNotify(GetWave(waveIndex).Lane);
            };
        }

        private VisualElement CreateRoadCell()
        {
            VisualElement cell = new();
            cell.AddToClassList(ROAD_CELL_CLASS);

            VisualElement swatch = new();
            swatch.AddToClassList(ROAD_SWATCH_CLASS);

            DropdownField dropdown = new(_roadChoices, 0);
            dropdown.AddToClassList(ROAD_DROPDOWN_CLASS);
            dropdown.RegisterValueChangedCallback(_ => OnRoadChosen(cell, dropdown));

            cell.Add(swatch);
            cell.Add(dropdown);

            return cell;
        }

        private void BindRoadCell(VisualElement cell, int waveIndex)
        {
            cell.userData = waveIndex;

            EnemiesWaveConfig wave = GetWave(waveIndex);
            int pathIndex = Array.IndexOf(_pathNumbers, wave.PathNumber);

            DropdownField dropdown = cell.Q<DropdownField>();
            dropdown.SetValueWithoutNotify(pathIndex >= 0 ? _roadChoices[pathIndex] : wave.PathNumber.ToString());
            dropdown.tooltip = "Дорога, по которой идёт волна";

            VisualElement swatch = cell.Q(className: ROAD_SWATCH_CLASS);

            for (int i = 0; i < _swatchClasses.Length; i++)
                swatch.EnableInClassList(_swatchClasses[i], i == pathIndex);
        }

        private void BindNumber(Label label, int waveIndex)
        {
            int waveNumber = waveIndex + 1;
            label.text = waveNumber.ToString();
        }

        private void BindComposition(Label label, int waveIndex)
        {
            EnemiesWaveConfig wave = GetWave(waveIndex);
            string composition = LevelEditorTexts.DescribeComposition(wave);

            label.text = composition;
            label.tooltip = string.IsNullOrEmpty(wave.Note) ? composition : $"{composition}\n{wave.Note}";
        }

        private void BindSpawnEnd(Label label, int waveIndex)
        {
            WaveStats stats = GetWaveStats(waveIndex);
            float stageTime = _session.Config.Stages[_stageIndex].StageTime;
            bool isLastStage = _stageIndex == _session.StagesCount - 1;
            bool isOverflow = isLastStage == false && LevelSliceNumbers.Round(stats.SpawnEnd) > LevelSliceNumbers.Round(stageTime);

            label.text = LevelEditorTexts.Seconds(stats.SpawnEnd);
            label.tooltip = isOverflow ? "Последний враг выходит позже конца стейджа" : "Когда выходит последний враг волны";
            label.EnableInClassList(WARNING_LABEL_CLASS, isOverflow);
        }

        private void OnRoadChosen(VisualElement cell, DropdownField dropdown)
        {
            int pathIndex = dropdown.index;

            if (pathIndex < 0 || pathIndex >= _pathNumbers.Length)
                return;

            _session.SetWavePathNumber(_stageIndex, (int)cell.userData, _pathNumbers[pathIndex]);
        }

        private void OnSelectedIndicesChanged(IEnumerable<int> indices)
        {
            int index = indices.DefaultIfEmpty(LevelAddress.NO_INDEX).First();

            if (index == LevelAddress.NO_INDEX)
            {
                CollectionViewIndices.Select(_table, _waveIndex);
                return;
            }

            _waveIndex = index;
            WaveSelected?.Invoke(index);
        }

        // Строка под курсором — свои действия, пустое место под таблицей — добавление в конец стейджа.
        // У текстовых ячеек своё меню (копирование текста) — его не дополняем.
        private void BuildWaveMenu(ContextualMenuPopulateEvent menuEvent)
        {
            if (IsStageSelected == false || CollectionViewIndices.IsInTextField(menuEvent.triggerEvent?.target, _table))
                return;

            DropdownMenu menu = menuEvent.menu;
            int stageIndex = _stageIndex;
            int waveIndex = CollectionViewIndices.FindClickedIndex(menuEvent.triggerEvent, _table);

            if (waveIndex == LevelAddress.NO_INDEX)
            {
                int wavesCount = _session.GetWavesCount(stageIndex);

                menu.AppendAction("Новая волна в конец", _ => _commands.InsertWave(stageIndex, wavesCount));
                menu.AppendAction("Вставить из буфера в конец", _ => _commands.Paste(stageIndex, LevelAddress.NO_INDEX));
                return;
            }

            SelectClickedWave(waveIndex);

            menu.AppendAction("Новая волна перед этой", _ => _commands.InsertWave(stageIndex, waveIndex));
            menu.AppendAction("Новая волна после этой", _ => _commands.InsertWave(stageIndex, waveIndex + 1));
            menu.AppendSeparator();
            menu.AppendAction("Дублировать", _ => _commands.DuplicateWave(stageIndex, waveIndex));
            menu.AppendAction("Копировать", _ => _commands.CopyWave(stageIndex, waveIndex));
            menu.AppendAction("Вставить из буфера", _ => _commands.Paste(stageIndex, waveIndex));
            AppendMoveToStageActions(menu, stageIndex, waveIndex);
            menu.AppendSeparator();
            menu.AppendAction("Удалить", _ => _commands.RemoveWave(stageIndex, waveIndex));
        }

        // Подменю со всеми стейджами, кроме текущего; если других стейджей нет — недоступный пункт.
        private void AppendMoveToStageActions(DropdownMenu menu, int stageIndex, int waveIndex)
        {
            bool hasTargetStages = false;

            for (int i = 0; i < _session.StagesCount; i++)
            {
                if (i == stageIndex)
                    continue;

                int targetStageIndex = i;
                string stageAddress = LevelAddress.Format(targetStageIndex);

                menu.AppendAction($"{MOVE_TO_STAGE_ITEM}/{stageAddress}", _ => _commands.MoveWaveToStage(stageIndex, waveIndex, targetStageIndex));
                hasTargetStages = true;
            }

            if (hasTargetStages == false)
                menu.AppendAction(MOVE_TO_STAGE_ITEM, _ => { }, DropdownMenuAction.Status.Disabled);
        }

        // Правый клик сам строку не выделяет — выделяем сразу, до показа меню. Отложить нельзя: пока меню открыто,
        // отложенные действия окна стоят и выполнились бы уже после выбранной команды, сбив её выбор.
        private void SelectClickedWave(int waveIndex)
        {
            if (waveIndex == _waveIndex)
                return;

            _waveIndex = waveIndex;
            CollectionViewIndices.Select(_table, waveIndex);
            WaveSelected?.Invoke(waveIndex);
        }

        // Таблица уже переставила строку у себя; данные переставляются на следующем кадре, вне события таблицы.
        private void OnWaveMoved(int waveIndex, int targetIndex)
        {
            int stageIndex = _stageIndex;
            _table.schedule.Execute(() => _commands.MoveWave(stageIndex, waveIndex, targetIndex));
        }

        private void OnAddWaveClicked()
        {
            if (IsStageSelected)
                _commands.InsertWave(_stageIndex, _session.GetWavesCount(_stageIndex));
        }

        private void CopySelectedWave()
        {
            if (IsWaveSelected)
                _commands.CopyWave(_stageIndex, _waveIndex);
        }

        // Стейджи из буфера встают после выбранного стейджа, волны — после выбранной волны.
        private void PasteAfterSelectedWave() => _commands.Paste(_stageIndex, _waveIndex);

        private void DuplicateSelectedWave()
        {
            if (IsWaveSelected)
                _commands.DuplicateWave(_stageIndex, _waveIndex);
        }

        private void RemoveSelectedWave()
        {
            if (IsWaveSelected)
                _commands.RemoveWave(_stageIndex, _waveIndex);
        }

        private static string GetSwatchClass(PathNumber pathNumber)
            => ROAD_SWATCH_CLASS_PREFIX + pathNumber.ToString().ToLowerInvariant();
    }
}
