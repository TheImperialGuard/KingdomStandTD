using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Окно редактора уровней: стейджи и волны LevelConfig, их статистика и замечания проверки, а на вкладке
    // «Все уровни» — итоги всех уровней и кривая сложности кампании. Окно запоминает уровень, стейдж, волну, вкладку,
    // режим таймлайна и показатель кривой сложности между открытиями.
    public class LevelEditorWindow : EditorWindow
    {
        private const string MENU_PATH = "Tools/Levels/Редактор уровней";
        private const int MENU_PRIORITY = 0;
        private const string WINDOW_TITLE = "Редактор уровней";
        private const string LAYOUT_FILE_NAME = "LevelEditorWindow.uxml";

        private const string PREFS_PREFIX = "KingdomStandTD.LevelEditor.";
        private const string LEVEL_PREF = PREFS_PREFIX + "Level";
        private const string STAGE_PREF = PREFS_PREFIX + "Stage";
        private const string WAVE_PREF = PREFS_PREFIX + "Wave";
        private const string TAB_PREF = PREFS_PREFIX + "Tab";
        private const string TIMELINE_MODE_PREF = PREFS_PREFIX + "TimelineMode";
        private const string DIFFICULTY_MEASURE_PREF = PREFS_PREFIX + "DifficultyMeasure";

        private const string HIDDEN_PAGE_CLASS = "level-editor__page--hidden";
        private const string HIDDEN_INDICATOR_CLASS = "toolbar__dirty-indicator--hidden";
        private const string LEVEL_FIELD_CLASS = "toolbar__level-field";

        private const int LEVEL_TAB = 0;
        private const int ALL_LEVELS_TAB = 1;

        private readonly List<LevelConfig> _levels = new();

        private LevelEditingSession _session;
        private LevelEditorCommands _commands;
        private EnemyCatalog _enemyCatalog;
        private LevelsListConfig _campaign;

        private PopupField<LevelConfig> _levelField;
        private ToolbarToggle _levelTab;
        private ToolbarToggle _allLevelsTab;
        private VisualElement _levelPageElement;
        private VisualElement _allLevelsPageElement;
        private Label _dirtyIndicator;
        private ToolbarButton _revertButton;
        private ToolbarButton _saveButton;

        private LevelSummaryBar _summaryBar;
        private StagesPanel _stagesPanel;
        private TimelinePanel _timelinePanel;
        private WavesTable _wavesTable;
        private WaveDetailsPanel _waveDetailsPanel;
        private IssuesPanel _issuesPanel;
        private AllLevelsPage _allLevelsPage;

        private int _stageIndex = LevelAddress.NO_INDEX;
        private int _waveIndex = LevelAddress.NO_INDEX;
        private int _tab = LEVEL_TAB;

        private bool IsAllLevelsTabSelected => _tab == ALL_LEVELS_TAB;

        [MenuItem(MENU_PATH, false, MENU_PRIORITY)]
        public static void Open() => GetWindow<LevelEditorWindow>(WINDOW_TITLE);

        // Открывает окно на вкладке уровня: из инспектора LevelConfig и по двойному щелчку на ассете.
        public static void Open(LevelConfig level)
        {
            LevelEditorWindow window = GetWindow<LevelEditorWindow>(WINDOW_TITLE);
            window.ShowLevel(level);
        }

        // Двойной щелчок по LevelConfig в окне Project открывает уровень в редакторе.
        [OnOpenAsset]
        private static bool OnOpenAsset(EntityId entityId)
        {
            if ((EditorUtility.EntityIdToObject(entityId) is LevelConfig level) == false)
                return false;

            Open(level);
            return true;
        }

        private void CreateGUI()
        {
            VisualTreeAsset layout = LevelEditorWindowFiles.LoadNextToScript<VisualTreeAsset>(this, LAYOUT_FILE_NAME);

            if (layout == null)
            {
                rootVisualElement.Add(new Label("Не найден макет окна " + LAYOUT_FILE_NAME));
                return;
            }

            layout.CloneTree(rootVisualElement);

            _enemyCatalog = new EnemyCatalog();
            _commands = new LevelEditorCommands(this);
            _commands.SelectionRequested += OnSelectionRequested;

            SetupToolbar();

            _summaryBar = new LevelSummaryBar(rootVisualElement);
            _stagesPanel = new StagesPanel(rootVisualElement, _enemyCatalog, _commands);
            _timelinePanel = new TimelinePanel(rootVisualElement, LoadTimelineMode());
            _wavesTable = new WavesTable(rootVisualElement, _commands);
            _waveDetailsPanel = new WaveDetailsPanel(rootVisualElement, _enemyCatalog);
            _issuesPanel = new IssuesPanel(rootVisualElement);
            _allLevelsPage = new AllLevelsPage(rootVisualElement, LoadDifficultyMeasure());

            _stagesPanel.StageSelected += OnStageSelected;
            _timelinePanel.WaveChosen += OnTimelineWaveChosen;
            _timelinePanel.StageChosen += OnStageSelected;
            _timelinePanel.ModeChanged += OnTimelineModeChanged;
            _wavesTable.WaveSelected += OnWaveSelected;
            _issuesPanel.IssueChosen += OnIssueChosen;
            _allLevelsPage.LevelChosen += ShowLevel;
            _allLevelsPage.StageChosen += OnOverviewStageChosen;
            _allLevelsPage.MeasureChanged += OnDifficultyMeasureChanged;

            EditorApplication.projectChanged += OnProjectChanged;

            ReloadLevels();
            OpenLevel(FindSavedLevel(), isRestoringSelection: true);
            SelectTab(EditorPrefs.GetInt(TAB_PREF, LEVEL_TAB));
        }

        // Правку конфига со стороны (инспектор, импорт среза) окно замечает здесь, несколько раз в секунду.
        private void OnInspectorUpdate()
        {
            _session?.ReloadIfChangedOutside();
            RefreshDirtyState();

            if (IsAllLevelsTabSelected)
                _allLevelsPage?.RefreshIfChangedOutside(_levels, _campaign);
        }

        // Пока окно было не в фокусе, могли измениться враги или префаб уровня.
        private void OnFocus() => _session?.Reload();

        private void OnDisable()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
            CloseSession();
        }

        private void SetupToolbar()
        {
            _levelField = new PopupField<LevelConfig>
            {
                formatSelectedValueCallback = FormatLevelName,
                formatListItemCallback = FormatLevelName,
                tooltip = "Уровни кампании по порядку, затем остальные LevelConfig",
            };

            _levelField.AddToClassList(LEVEL_FIELD_CLASS);
            _levelField.RegisterValueChangedCallback(changeEvent => OpenLevel(changeEvent.newValue, isRestoringSelection: false));
            rootVisualElement.Q<VisualElement>("levelSelectorSlot").Add(_levelField);

            _levelTab = rootVisualElement.Q<ToolbarToggle>("levelTab");
            _allLevelsTab = rootVisualElement.Q<ToolbarToggle>("allLevelsTab");
            _levelTab.RegisterValueChangedCallback(_ => SelectTab(LEVEL_TAB));
            _allLevelsTab.RegisterValueChangedCallback(_ => SelectTab(ALL_LEVELS_TAB));

            _levelPageElement = rootVisualElement.Q<VisualElement>("levelPage");
            _allLevelsPageElement = rootVisualElement.Q<VisualElement>("allLevelsPage");

            rootVisualElement.Q<ToolbarButton>("validateAllButton").clicked += OnValidateAllClicked;
            rootVisualElement.Q<ToolbarButton>("copyLevelButton").clicked += OnCopyLevelClicked;
            rootVisualElement.Q<ToolbarButton>("pasteButton").clicked += OnPasteClicked;
            rootVisualElement.Q<ToolbarButton>("bulkEditButton").clicked += OnBulkEditClicked;

            _revertButton = rootVisualElement.Q<ToolbarButton>("revertButton");
            _revertButton.clicked += OnRevertClicked;
            _saveButton = rootVisualElement.Q<ToolbarButton>("saveButton");
            _saveButton.clicked += OnSaveClicked;
            _dirtyIndicator = rootVisualElement.Q<Label>("dirtyIndicator");
        }

        private void ReloadLevels()
        {
            _campaign = LevelEditorAssets.FindLevelsList();

            _levels.Clear();
            _levels.AddRange(LevelEditorAssets.FindLevelConfigs());

            _levelField.choices = _levels;
        }

        // Только что созданное окно строит интерфейс позже, в CreateGUI: тогда уровень запоминается,
        // и CreateGUI откроет его как сохранённый выбор. Уже открытый уровень не переоткрывается, выбор в нём остаётся.
        private void ShowLevel(LevelConfig level)
        {
            if (_levelField == null)
            {
                EditorPrefs.SetString(LEVEL_PREF, GetAssetGuid(level));
                EditorPrefs.SetInt(TAB_PREF, LEVEL_TAB);
                SetSelection(0, 0);
                return;
            }

            if (_levels.Contains(level) == false)
                ReloadLevels();

            bool isAlreadyOpen = _session != null && _session.Config == level;

            if (isAlreadyOpen == false)
                OpenLevel(level, isRestoringSelection: false);

            SelectTab(LEVEL_TAB);
        }

        private void OpenLevel(LevelConfig level, bool isRestoringSelection)
        {
            CloseSession();

            if ((level == null || _levels.Contains(level) == false) && _levels.Count > 0)
                level = _levels[0];

            _levelPageElement.SetEnabled(level != null);

            if (level == null)
                return;

            _session = new LevelEditingSession(level);
            _session.Changed += OnSessionChanged;

            _commands.Bind(_session);
            _summaryBar.Bind(_session);
            _stagesPanel.Bind(_session);
            _timelinePanel.Bind(_session);
            _wavesTable.Bind(_session);
            _waveDetailsPanel.Bind(_session);

            _levelField.SetValueWithoutNotify(level);
            EditorPrefs.SetString(LEVEL_PREF, GetAssetGuid(level));

            _stageIndex = isRestoringSelection ? EditorPrefs.GetInt(STAGE_PREF, 0) : 0;
            _waveIndex = isRestoringSelection ? EditorPrefs.GetInt(WAVE_PREF, 0) : 0;

            RefreshAll();
        }

        private void CloseSession()
        {
            if (_session == null)
                return;

            _session.Changed -= OnSessionChanged;
            _session.Dispose();
            _session = null;

            _commands?.Bind(null);
        }

        private void RefreshAll()
        {
            if (_session == null || _session.IsAlive == false)
                return;

            ClampSelection();

            _summaryBar.Refresh();
            _stagesPanel.Refresh(_stageIndex);
            _timelinePanel.Refresh(_stageIndex, _waveIndex);
            _wavesTable.Refresh(_stageIndex, _waveIndex);
            _waveDetailsPanel.Refresh(_stageIndex, _waveIndex);
            _issuesPanel.Refresh(_session.Issues);

            RefreshDirtyState();
            RefreshAllLevelsPage();
        }

        // Итоги всех уровней пересчитываются, только пока их вкладка открыта.
        private void RefreshAllLevelsPage()
        {
            if (IsAllLevelsTabSelected)
                _allLevelsPage.Refresh(_levels, _campaign);
        }

        // Выбор держится в пределах уровня: после удаления или Undo стейджа и волны индексы сдвигаются к ближайшим.
        private void ClampSelection()
        {
            int stagesCount = _session.StagesCount;

            if (stagesCount == 0)
            {
                _stageIndex = LevelAddress.NO_INDEX;
                _waveIndex = LevelAddress.NO_INDEX;
                return;
            }

            _stageIndex = Mathf.Clamp(_stageIndex, 0, stagesCount - 1);

            int wavesCount = _session.GetWavesCount(_stageIndex);
            _waveIndex = wavesCount == 0 ? LevelAddress.NO_INDEX : Mathf.Clamp(_waveIndex, 0, wavesCount - 1);
        }

        private void RefreshDirtyState()
        {
            if (_dirtyIndicator == null)
                return;

            bool isDirty = _session != null && _session.IsDirty;

            _dirtyIndicator.EnableInClassList(HIDDEN_INDICATOR_CLASS, isDirty == false);
            _revertButton.SetEnabled(isDirty);
            _saveButton.SetEnabled(isDirty);
        }

        private void SelectTab(int tab)
        {
            bool isLevelTab = tab != ALL_LEVELS_TAB;
            _tab = isLevelTab ? LEVEL_TAB : ALL_LEVELS_TAB;

            _levelTab.SetValueWithoutNotify(isLevelTab);
            _allLevelsTab.SetValueWithoutNotify(isLevelTab == false);
            _levelPageElement.EnableInClassList(HIDDEN_PAGE_CLASS, isLevelTab == false);
            _allLevelsPageElement.EnableInClassList(HIDDEN_PAGE_CLASS, isLevelTab);

            EditorPrefs.SetInt(TAB_PREF, _tab);
            RefreshAllLevelsPage();
        }

        private void SetSelection(int stageIndex, int waveIndex)
        {
            _stageIndex = stageIndex;
            _waveIndex = waveIndex;

            EditorPrefs.SetInt(STAGE_PREF, _stageIndex);
            EditorPrefs.SetInt(WAVE_PREF, _waveIndex);
        }

        private static TimelineMode LoadTimelineMode()
        {
            int savedMode = EditorPrefs.GetInt(TIMELINE_MODE_PREF, (int)TimelineMode.Stage);

            return Enum.IsDefined(typeof(TimelineMode), savedMode) ? (TimelineMode)savedMode : TimelineMode.Stage;
        }

        private static DifficultyMeasure LoadDifficultyMeasure()
        {
            int savedMeasure = EditorPrefs.GetInt(DIFFICULTY_MEASURE_PREF, (int)DifficultyMeasure.Health);

            return Enum.IsDefined(typeof(DifficultyMeasure), savedMeasure) ? (DifficultyMeasure)savedMeasure : DifficultyMeasure.Health;
        }

        private LevelConfig FindSavedLevel()
        {
            string guid = EditorPrefs.GetString(LEVEL_PREF, string.Empty);

            if (string.IsNullOrEmpty(guid))
                return null;

            string path = AssetDatabase.GUIDToAssetPath(guid);

            return AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
        }

        private string FormatLevelName(LevelConfig level)
        {
            if (level == null)
                return "—";

            int? campaignNumber = LevelEditorAssets.GetCampaignNumber(_campaign, level);

            if (campaignNumber.HasValue)
                return $"{campaignNumber.Value}. {level.name}";

            return $"{level.name} (вне кампании)";
        }

        private static string GetAssetGuid(LevelConfig level) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(level));

        private void OnSessionChanged()
        {
            if (_session.IsAlive == false)
            {
                ReloadLevels();
                OpenLevel(null, isRestoringSelection: false);
                return;
            }

            RefreshAll();
        }

        private void OnProjectChanged()
        {
            ReloadLevels();
            _enemyCatalog.Reload();

            if (_session == null || _session.IsAlive == false)
            {
                OpenLevel(null, isRestoringSelection: false);
                return;
            }

            _session.Reload();
        }

        private void OnStageSelected(int stageIndex)
        {
            if (stageIndex == _stageIndex)
                return;

            SetSelection(stageIndex, 0);
            RefreshAll();
        }

        private void OnWaveSelected(int waveIndex)
        {
            if (waveIndex == _waveIndex)
                return;

            SetSelection(_stageIndex, waveIndex);

            _waveDetailsPanel.Refresh(_stageIndex, _waveIndex);
            _timelinePanel.Refresh(_stageIndex, _waveIndex);
        }

        private void OnTimelineWaveChosen(int stageIndex, int waveIndex)
        {
            SetSelection(stageIndex, waveIndex);
            RefreshAll();
        }

        private void OnTimelineModeChanged(TimelineMode mode) => EditorPrefs.SetInt(TIMELINE_MODE_PREF, (int)mode);

        private void OnDifficultyMeasureChanged(DifficultyMeasure measure) => EditorPrefs.SetInt(DIFFICULTY_MEASURE_PREF, (int)measure);

        // Щелчок по столбику кривой сложности: уровень открывается на вкладке «Уровень» сразу на этом стейдже.
        private void OnOverviewStageChosen(LevelConfig level, int stageIndex)
        {
            ShowLevel(level);
            SetSelection(stageIndex, 0);
            RefreshAll();
        }

        // Команда выбирает элемент до правки, а окно перерисуется уже по событию правки — вместе с новым выбором.
        private void OnSelectionRequested(int stageIndex, int waveIndex) => SetSelection(stageIndex, waveIndex);

        private void OnIssueChosen(LevelIssue issue)
        {
            if (issue.StageIndex == LevelAddress.NO_INDEX)
                return;

            int waveIndex = issue.WaveIndex != LevelAddress.NO_INDEX ? issue.WaveIndex : 0;

            SetSelection(issue.StageIndex, waveIndex);
            RefreshAll();
        }

        private void OnValidateAllClicked()
        {
            LevelBalanceCommands.LogValidationReport();
            _session?.Reload();
        }

        private void OnCopyLevelClicked() => _commands.CopyLevel();

        private void OnPasteClicked() => _commands.Paste(_stageIndex, _waveIndex);

        private void OnBulkEditClicked() => _commands.EditInBulk(_stageIndex);

        private void OnRevertClicked() => _session?.RevertToSaved();

        private void OnSaveClicked() => _session?.Save();
    }
}
