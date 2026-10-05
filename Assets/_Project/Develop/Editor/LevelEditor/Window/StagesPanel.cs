using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Левая панель: список стейджей, параметры и статистика выбранного стейджа, его заметка.
    // Стейдж добавляется кнопкой и контекстным меню, переставляется перетаскиванием; копирование, вставка,
    // дублирование и удаление — в меню и командами Ctrl+C, Ctrl+V, Ctrl+D и Del.
    public class StagesPanel
    {
        private const string STAGE_ROW_CLASS = "stage-row";
        private const string MISSING_VALUE = "—";

        public event Action<int> StageSelected;

        private readonly EnemyCatalog _enemyCatalog;
        private readonly LevelEditorCommands _commands;
        private readonly ListView _stagesList;
        private readonly VisualElement _stageContent;
        private readonly Label _stageTitle;
        private readonly FloatField _stageTimeField;
        private readonly FloatField _skipTimeField;
        private readonly TextField _noteField;
        private readonly StatsSheet _enemiesSheet;
        private readonly StatsSheet _economySheet;
        private readonly StatsSheet _threatSheet;
        private readonly List<int> _stageIndices = new();

        private LevelEditingSession _session;
        private int _stageIndex = LevelAddress.NO_INDEX;

        public StagesPanel(VisualElement root, EnemyCatalog enemyCatalog, LevelEditorCommands commands)
        {
            _enemyCatalog = enemyCatalog;
            _commands = commands;

            _stagesList = root.Q<ListView>("stagesList");
            _stagesList.itemsSource = _stageIndices;
            _stagesList.makeItem = CreateStageRow;
            _stagesList.bindItem = BindStageRow;
            _stagesList.selectedIndicesChanged += OnSelectedIndicesChanged;
            _stagesList.itemIndexChanged += OnStageMoved;
            _stagesList.Q<ScrollView>().AddManipulator(new ContextualMenuManipulator(BuildStageMenu));

            CollectionViewCommands.Register(_stagesList, CopySelectedStage, PasteAfterSelectedStage, DuplicateSelectedStage, RemoveSelectedStage);
            root.Q<Button>("addStageButton").clicked += OnAddStageClicked;

            _stageContent = root.Q<VisualElement>("stageContent");
            _stageTitle = root.Q<Label>("stageTitle");
            _stageTimeField = root.Q<FloatField>("stageTimeField");
            _skipTimeField = root.Q<FloatField>("skipTimeField");
            _noteField = root.Q<TextField>("stageNoteField");

            _enemiesSheet = new StatsSheet(root.Q<VisualElement>("enemiesStats"));
            _economySheet = new StatsSheet(root.Q<VisualElement>("economyStats"));
            _threatSheet = new StatsSheet(root.Q<VisualElement>("threatStats"));

            _stageTimeField.RegisterValueChangedCallback(changeEvent => OnStageTimeChanged(changeEvent.newValue));
            _skipTimeField.RegisterValueChangedCallback(changeEvent => OnSkipTimeChanged(changeEvent.newValue));
            _noteField.RegisterValueChangedCallback(changeEvent => OnNoteChanged(changeEvent.newValue));
        }

        private bool IsStageSelected => _session != null && _stageIndex != LevelAddress.NO_INDEX;

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex)
        {
            bool isSelectionChanged = stageIndex != _stageIndex;
            _stageIndex = stageIndex;

            CollectionViewIndices.Sync(_stagesList, _stageIndices, _session.StagesCount);
            CollectionViewIndices.Select(_stagesList, stageIndex);

            if (isSelectionChanged && stageIndex != LevelAddress.NO_INDEX)
                _stagesList.ScrollToItem(stageIndex);

            _stageContent.SetEnabled(IsStageSelected);

            if (IsStageSelected == false)
            {
                _stageTitle.text = "Стейдж не выбран";
                ClearStats();
                return;
            }

            EnemiesWavesStageConfig stage = _session.Config.Stages[stageIndex];
            StageStats stats = _session.Stats.Stages[stageIndex];
            bool isLastStage = stageIndex == _session.StagesCount - 1;

            _stageTitle.text = LevelAddress.Format(stageIndex);
            _stageTimeField.SetValueWithoutNotify(stage.StageTime);
            _skipTimeField.SetValueWithoutNotify(stage.TimeToSkipStage);
            _noteField.SetValueWithoutNotify(stage.Note ?? string.Empty);

            RefreshStats(stats, isLastStage);
        }

        private VisualElement CreateStageRow()
        {
            Label row = new();
            row.AddToClassList(STAGE_ROW_CLASS);

            return row;
        }

        private void BindStageRow(VisualElement element, int stageIndex)
        {
            Label row = (Label)element;
            EnemiesWavesStageConfig stage = _session.Config.Stages[stageIndex];
            StageStats stats = _session.Stats.Stages[stageIndex];

            row.userData = stageIndex;
            row.text = LevelEditorTexts.DescribeStageRow(stageIndex, stage, stats, _session.Issues);
            row.tooltip = stage.Note;
        }

        // Строка под курсором — свои действия, пустое место под списком — добавление в конец.
        private void BuildStageMenu(ContextualMenuPopulateEvent menuEvent)
        {
            if (_session == null)
                return;

            DropdownMenu menu = menuEvent.menu;
            int stageIndex = CollectionViewIndices.FindClickedIndex(menuEvent.triggerEvent, _stagesList);

            if (stageIndex == LevelAddress.NO_INDEX)
            {
                int stagesCount = _session.StagesCount;
                int lastStageIndex = stagesCount - 1;

                menu.AppendAction("Новый стейдж в конец", _ => _commands.InsertStage(stagesCount));
                menu.AppendAction("Вставить из буфера в конец", _ => _commands.Paste(lastStageIndex, LevelAddress.NO_INDEX));
                return;
            }

            SelectClickedStage(stageIndex);

            menu.AppendAction("Новый стейдж перед этим", _ => _commands.InsertStage(stageIndex));
            menu.AppendAction("Новый стейдж после этого", _ => _commands.InsertStage(stageIndex + 1));
            menu.AppendSeparator();
            menu.AppendAction("Дублировать", _ => _commands.DuplicateStage(stageIndex));
            menu.AppendAction("Копировать", _ => _commands.CopyStage(stageIndex));
            menu.AppendAction("Вставить из буфера", _ => _commands.Paste(stageIndex, LevelAddress.NO_INDEX));
            menu.AppendSeparator();
            menu.AppendAction("Удалить", _ => _commands.RemoveStage(stageIndex));
        }

        // Правый клик сам строку не выделяет — выделяем сразу, до показа меню. Отложить нельзя: пока меню открыто,
        // отложенные действия окна стоят и выполнились бы уже после выбранной команды, сбив её выбор.
        private void SelectClickedStage(int stageIndex)
        {
            if (stageIndex != _stageIndex)
                StageSelected?.Invoke(stageIndex);
        }

        private void RefreshStats(StageStats stats, bool isLastStage)
        {
            ClearStats();

            _enemiesSheet.AddRow("Врагов", stats.EnemiesCount.ToString());

            foreach (KeyValuePair<string, int> enemyCount in stats.EnemiesByType.OrderByDescending(enemyCount => enemyCount.Value))
                _enemiesSheet.AddNestedRow(_enemyCatalog.GetDisplayNameByAsset(enemyCount.Key), enemyCount.Value.ToString());

            _enemiesSheet.AddRow("HP", LevelEditorTexts.Number(stats.Health));
            _enemiesSheet.AddRow(
                "EHP против физики",
                LevelEditorTexts.Number(stats.EffectiveHealthPhysic),
                LevelEditorTexts.DescribeMeasure(DifficultyMeasure.EffectiveHealthPhysic));
            _enemiesSheet.AddRow(
                "EHP против магии",
                LevelEditorTexts.Number(stats.EffectiveHealthMagic),
                LevelEditorTexts.DescribeMeasure(DifficultyMeasure.EffectiveHealthMagic));

            string skipTooltip = isLastStage ? "В последнем стейдже скипа нет" : "Если скипнуть, как только это разрешено";
            string goldAtStart = $"{stats.GoldAtStartWithoutSkips} (со скипами {stats.GoldAtStartWithMaxSkips})";

            _economySheet.AddRow("Золото за убийства", stats.KillGold.ToString());
            _economySheet.AddRow("Максимум за скип", stats.MaxSkipGold.ToString(), skipTooltip);
            _economySheet.AddRow(
                "Золото к началу",
                goldAtStart,
                "Стартовое золото и награды прошлых стейджей, будто все их враги убиты");

            string threat = LevelEditorTexts.Number(stats.BreakthroughThreat);
            string threatText = $"×{threat} (урон {stats.BreakthroughDamage})";
            string peak = stats.PeakEnemies.HasValue ? stats.PeakEnemies.Value.ToString() : MISSING_VALUE;
            string lastArrival = stats.LastArrival.HasValue ? LevelEditorTexts.Seconds(stats.LastArrival.Value) : MISSING_VALUE;
            bool isOverflow = isLastStage == false && stats.SpawnOverflow > 0f;
            string spawnEnd = LevelEditorTexts.Seconds(stats.SpawnEnd);
            string spawnEndText = isOverflow ? $"{spawnEnd} — позже конца стейджа" : spawnEnd;

            _threatSheet.AddRow(
                "HP в секунду",
                LevelEditorTexts.Number(stats.HealthPerSecond),
                LevelEditorTexts.DescribeMeasure(DifficultyMeasure.HealthPerSecond));
            _threatSheet.AddRow(
                "Угроза прорыва",
                threatText,
                "Урон по базе, если прорвутся все враги стейджа, в долях здоровья игрока");
            _threatSheet.AddRow("Пик врагов на поле", peak, "Оценка без убийств: враг на поле от спавна до базы");
            _threatSheet.AddRow("Конец спавна", spawnEndText, "Когда выходит последний враг стейджа", isOverflow);
            _threatSheet.AddRow("Последний приход на базу", lastArrival, "Оценка без убийств, от начала стейджа");
        }

        private void ClearStats()
        {
            _enemiesSheet.Clear();
            _economySheet.Clear();
            _threatSheet.Clear();
        }

        private void OnSelectedIndicesChanged(IEnumerable<int> indices)
        {
            int index = indices.DefaultIfEmpty(LevelAddress.NO_INDEX).First();

            // У окна стейдж выбран всегда: от него зависят остальные панели. Если список потерял выделение — возвращаем.
            if (index == LevelAddress.NO_INDEX)
            {
                CollectionViewIndices.Select(_stagesList, _stageIndex);
                return;
            }

            StageSelected?.Invoke(index);
        }

        // Список уже переставил строку у себя; данные переставляются на следующем кадре, вне события списка.
        private void OnStageMoved(int stageIndex, int targetIndex)
            => _stagesList.schedule.Execute(() => _commands.MoveStage(stageIndex, targetIndex));

        private void OnAddStageClicked()
        {
            if (_session != null)
                _commands.InsertStage(_session.StagesCount);
        }

        private void CopySelectedStage()
        {
            if (IsStageSelected)
                _commands.CopyStage(_stageIndex);
        }

        // Волны из буфера встают в конец выбранного стейджа.
        private void PasteAfterSelectedStage() => _commands.Paste(_stageIndex, LevelAddress.NO_INDEX);

        private void DuplicateSelectedStage()
        {
            if (IsStageSelected)
                _commands.DuplicateStage(_stageIndex);
        }

        private void RemoveSelectedStage()
        {
            if (IsStageSelected)
                _commands.RemoveStage(_stageIndex);
        }

        private void OnStageTimeChanged(float value)
        {
            if (IsStageSelected)
                _session.SetStageTime(_stageIndex, value);
        }

        private void OnSkipTimeChanged(float value)
        {
            if (IsStageSelected)
                _session.SetTimeToSkipStage(_stageIndex, value);
        }

        private void OnNoteChanged(string value)
        {
            if (IsStageSelected)
                _session.SetStageNote(_stageIndex, value);
        }
    }
}
