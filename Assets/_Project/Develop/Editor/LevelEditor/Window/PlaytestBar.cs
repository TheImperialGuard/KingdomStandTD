using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Editor.EndlessMode;
using Assets._Project.Develop.Editor.LevelEditor.Playtest;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.GameMode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Строка «Плейтест» под строкой уровня: запуск уровня в Play Mode целиком или с выбранного стейджа. Золото для запуска
    // со стейджа по умолчанию — расчёт к его началу без скипов; поправленное руками число держится, пока выбран тот же
    // стейдж. Во время игры строка показывает, что именно играется и какой стейдж идёт.
    //
    // Бесконечный уровень своих стейджей не имеет и запускается с волны из поля «Волна» — это то же значение, что «Старт с
    // волны» в инспекторе конфига режима. Золото по умолчанию — расчёт к началу этой волны без скипов.
    public class PlaytestBar
    {
        private const int FIRST_STAGE_INDEX = 0;
        private const string PLAY_FROM_STAGE_TEXT = "▶ Со стейджа";
        private const string PLAY_FROM_WAVE_TEXT = "▶ С волны";
        private const string PLAY_FROM_WAVE_TOOLTIP = "Запустить бесконечный уровень с волны из поля «Волна», золото на "
            + "старте — из поля справа. В игре счёт волн начнётся с этой волны, рекорд не записывается";

        private readonly Button _playButton;
        private readonly Button _playFromStageButton;
        private readonly IntegerField _waveField;
        private readonly IntegerField _goldField;
        private readonly Label _statusLabel;
        private readonly string _playFromStageTooltip;

        private LevelEditingSession _session;
        private int _stageIndex = LevelAddress.NO_INDEX;

        private LevelConfig _goldLevel;
        private int _goldStartIndex = LevelAddress.NO_INDEX;
        private bool _isGoldEdited;
        private bool _hasEndlessErrors;

        private bool _isPlayingShown;
        private PlaytestLaunch _launchShown;
        private int _stageNumberShown;

        public PlaytestBar(VisualElement root)
        {
            _playButton = root.Q<Button>("playButton");
            _playFromStageButton = root.Q<Button>("playFromStageButton");
            _waveField = root.Q<IntegerField>("playtestWaveField");
            _goldField = root.Q<IntegerField>("playtestGoldField");
            _statusLabel = root.Q<Label>("playtestStatusLabel");
            _playFromStageTooltip = _playFromStageButton.tooltip;

            _playButton.clicked += OnPlayClicked;
            _playFromStageButton.clicked += OnPlayFromStageClicked;
            _waveField.RegisterValueChangedCallback(changeEvent => OnWaveChanged(changeEvent.newValue));
            _goldField.RegisterValueChangedCallback(_ => _isGoldEdited = true);
        }

        private bool HasSelectedStage => _session != null && _stageIndex != LevelAddress.NO_INDEX;

        private bool IsEndless => _session != null && _session.Config.GameMode == GameModes.Infinity;

        // Откуда запускать не с начала: у кампании — выбранный стейдж, у бесконечного — волна из поля
        private bool HasStartPoint => IsEndless || HasSelectedStage;

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex)
        {
            _stageIndex = stageIndex;

            RefreshWave();
            RefreshGold();
            RefreshState();
        }

        // Play Mode и плейтест меняются вне окна, поэтому окно сверяет их несколько раз в секунду. Стартовую волну
        // бесконечного уровня могут поменять и в инспекторе конфига режима.
        public void RefreshPlayState()
        {
            if (IsEndless && _waveField.value != EndlessModeEditorSettings.StartWaveNumber)
            {
                RefreshWave();
                RefreshGold();
                RefreshState();
                return;
            }

            bool isPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
            bool isSameLaunch = PlaytestLauncher.RunningLaunch == _launchShown;
            bool isSameStage = PlaytestLauncher.RunningStageNumber == _stageNumberShown;

            if (isPlaying == _isPlayingShown && isSameLaunch && isSameStage)
                return;

            RefreshState();
        }

        private void RefreshWave()
        {
            _waveField.style.display = IsEndless ? DisplayStyle.Flex : DisplayStyle.None;
            _waveField.SetValueWithoutNotify(EndlessModeEditorSettings.StartWaveNumber);
        }

        private void RefreshGold()
        {
            _hasEndlessErrors = false;

            if (IsEndless)
            {
                RefreshEndlessGold();
                return;
            }

            _playFromStageButton.tooltip = _playFromStageTooltip;

            if (HasSelectedStage == false)
            {
                _playFromStageButton.text = PLAY_FROM_STAGE_TEXT;
                return;
            }

            int stageNumber = _stageIndex + 1;
            StageStats stats = _session.Stats.Stages[_stageIndex];

            _playFromStageButton.text = $"{PLAY_FROM_STAGE_TEXT} {stageNumber}";

            ResetGoldEditOnMove(_stageIndex);

            if (_isGoldEdited == false)
                _goldField.SetValueWithoutNotify(stats.GoldAtStartWithoutSkips);

            _goldField.tooltip = $"Золото на старте при запуске со стейджа {stageNumber}. По расчёту к его началу — "
                + $"{stats.GoldAtStartWithoutSkips} без скипов и {stats.GoldAtStartWithMaxSkips} со скипами, будто враги "
                + "прошлых стейджей убиты. Число можно поправить: оно держится, пока выбран этот стейдж";
        }

        // Волны собирает генератор режима; с ошибками в его конфиге их не собрать — и бой бы не запустился
        private void RefreshEndlessGold()
        {
            int waveNumber = EndlessModeEditorSettings.StartWaveNumber;

            _playFromStageButton.text = $"{PLAY_FROM_WAVE_TEXT} {waveNumber}";
            _playFromStageButton.tooltip = PLAY_FROM_WAVE_TOOLTIP;

            ResetGoldEditOnMove(waveNumber - EndlessEditorWaves.FirstWaveNumber);

            EndlessModeConfig config = LevelEditorAssets.FindEndlessModeConfig(_session.Config);
            List<LevelIssue> issues = new();

            if (config == null || EndlessEditorWaves.TryCreateGenerator(config, issues, out EndlessWavesGenerator generator) == false)
            {
                _hasEndlessErrors = true;
                return;
            }

            int gold = EndlessEditorWaves.CalculateGoldAtStart(_session.Config, generator, waveNumber);

            if (_isGoldEdited == false)
                _goldField.SetValueWithoutNotify(gold);

            _goldField.tooltip = $"Золото на старте при запуске с волны {waveNumber}. По расчёту к её началу без скипов — "
                + $"{gold}, будто враги прошлых волн убиты. Число можно поправить: оно держится, пока выбрана эта волна";
        }

        // Поправленное руками золото сбрасывается, когда меняется уровень или точка старта
        private void ResetGoldEditOnMove(int startIndex)
        {
            if (_session.Config == _goldLevel && startIndex == _goldStartIndex)
                return;

            _goldLevel = _session.Config;
            _goldStartIndex = startIndex;
            _isGoldEdited = false;
        }

        private void RefreshState()
        {
            _isPlayingShown = EditorApplication.isPlayingOrWillChangePlaymode;
            _launchShown = PlaytestLauncher.RunningLaunch;
            _stageNumberShown = PlaytestLauncher.RunningStageNumber;

            bool hasErrors = _session != null && _session.Issues.Any(issue => issue.IsError);
            bool canLaunch = _session != null && _isPlayingShown == false && hasErrors == false && _hasEndlessErrors == false;

            _playButton.SetEnabled(canLaunch);
            _playFromStageButton.SetEnabled(canLaunch && HasStartPoint);
            _waveField.SetEnabled(_isPlayingShown == false);
            _goldField.SetEnabled(_isPlayingShown == false && HasStartPoint);

            _statusLabel.text = DescribeState(hasErrors);
        }

        private string DescribeState(bool hasErrors)
        {
            if (_launchShown != null)
                return DescribeLaunch(_launchShown, _stageNumberShown);

            if (_isPlayingShown)
                return "Идёт игра — плейтест можно запустить после выхода из Play Mode";

            if (hasErrors)
                return "Запуск недоступен: исправьте ошибки уровня (✖)";

            if (_hasEndlessErrors)
                return "Запуск недоступен: исправьте ошибки конфига бесконечного режима — они видны в его инспекторе";

            return string.Empty;
        }

        // При старте не с первого стейджа счёт волн в игре сдвинут — строка подсказывает и игровой номер. У бесконечного
        // уровня номер волны в игре настоящий.
        private static string DescribeLaunch(PlaytestLaunch launch, int stageNumber)
        {
            string text = $"Идёт плейтест: {launch.Level.name}";

            if (launch.StartGold.HasValue)
                text += $", золото на старте {launch.StartGold.Value}";

            if (stageNumber == PlaytestLogService.NOT_STARTED_STAGE_NUMBER)
                return text + " · подготовка";

            if (launch.IsEndless)
                return text + $" · волна {stageNumber}";

            int stagesCount = launch.Level.Stages.Count;
            text += $" · стейдж {stageNumber} из {stagesCount}";

            if (launch.StartStageIndex == FIRST_STAGE_INDEX)
                return text;

            int gameWaveNumber = stageNumber - launch.StartStageIndex;

            return text + $" (в игре «Волна {gameWaveNumber}»)";
        }

        private void OnWaveChanged(int waveNumber)
        {
            EndlessModeEditorSettings.StartWaveNumber = waveNumber;

            RefreshWave();
            RefreshGold();
            RefreshState();
        }

        private void OnPlayClicked()
        {
            PlaytestLauncher.Launch(new PlaytestLaunch(_session.Config, FIRST_STAGE_INDEX, startGold: null));
        }

        private void OnPlayFromStageClicked()
        {
            int startGold = Mathf.Max(LevelConfigLimits.MIN_START_GOLD, _goldField.value);
            int startIndex = IsEndless ? EndlessModeEditorSettings.StartWaveNumber - EndlessEditorWaves.FirstWaveNumber : _stageIndex;

            PlaytestLauncher.Launch(new PlaytestLaunch(_session.Config, startIndex, startGold));
        }
    }
}
