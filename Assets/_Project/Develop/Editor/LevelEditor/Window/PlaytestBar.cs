using System.Linq;
using Assets._Project.Develop.Editor.LevelEditor.Playtest;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Строка «Плейтест» под строкой уровня: запуск уровня в Play Mode целиком или с выбранного стейджа. Золото для запуска
    // со стейджа по умолчанию — расчёт к его началу без скипов; поправленное руками число держится, пока выбран тот же
    // стейдж. Во время игры строка показывает, что именно играется и какой стейдж идёт.
    public class PlaytestBar
    {
        private const int FIRST_STAGE_INDEX = 0;
        private const string PLAY_FROM_STAGE_TEXT = "▶ Со стейджа";

        private readonly Button _playButton;
        private readonly Button _playFromStageButton;
        private readonly IntegerField _goldField;
        private readonly Label _statusLabel;

        private LevelEditingSession _session;
        private int _stageIndex = LevelAddress.NO_INDEX;

        private LevelConfig _goldLevel;
        private int _goldStageIndex = LevelAddress.NO_INDEX;
        private bool _isGoldEdited;

        private bool _isPlayingShown;
        private PlaytestLaunch _launchShown;
        private int _stageNumberShown;

        public PlaytestBar(VisualElement root)
        {
            _playButton = root.Q<Button>("playButton");
            _playFromStageButton = root.Q<Button>("playFromStageButton");
            _goldField = root.Q<IntegerField>("playtestGoldField");
            _statusLabel = root.Q<Label>("playtestStatusLabel");

            _playButton.clicked += OnPlayClicked;
            _playFromStageButton.clicked += OnPlayFromStageClicked;
            _goldField.RegisterValueChangedCallback(_ => _isGoldEdited = true);
        }

        private bool HasSelectedStage => _session != null && _stageIndex != LevelAddress.NO_INDEX;

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh(int stageIndex)
        {
            _stageIndex = stageIndex;

            RefreshGold();
            RefreshState();
        }

        // Play Mode и плейтест меняются вне окна, поэтому окно сверяет их несколько раз в секунду.
        public void RefreshPlayState()
        {
            bool isPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
            bool isSameLaunch = PlaytestLauncher.RunningLaunch == _launchShown;
            bool isSameStage = PlaytestLauncher.RunningStageNumber == _stageNumberShown;

            if (isPlaying == _isPlayingShown && isSameLaunch && isSameStage)
                return;

            RefreshState();
        }

        private void RefreshGold()
        {
            if (HasSelectedStage == false)
            {
                _playFromStageButton.text = PLAY_FROM_STAGE_TEXT;
                return;
            }

            int stageNumber = _stageIndex + 1;
            StageStats stats = _session.Stats.Stages[_stageIndex];

            _playFromStageButton.text = $"{PLAY_FROM_STAGE_TEXT} {stageNumber}";

            if (_session.Config != _goldLevel || _stageIndex != _goldStageIndex)
            {
                _goldLevel = _session.Config;
                _goldStageIndex = _stageIndex;
                _isGoldEdited = false;
            }

            if (_isGoldEdited == false)
                _goldField.SetValueWithoutNotify(stats.GoldAtStartWithoutSkips);

            _goldField.tooltip = $"Золото на старте при запуске со стейджа {stageNumber}. По расчёту к его началу — "
                + $"{stats.GoldAtStartWithoutSkips} без скипов и {stats.GoldAtStartWithMaxSkips} со скипами, будто враги "
                + "прошлых стейджей убиты. Число можно поправить: оно держится, пока выбран этот стейдж";
        }

        private void RefreshState()
        {
            _isPlayingShown = EditorApplication.isPlayingOrWillChangePlaymode;
            _launchShown = PlaytestLauncher.RunningLaunch;
            _stageNumberShown = PlaytestLauncher.RunningStageNumber;

            bool hasErrors = _session != null && _session.Issues.Any(issue => issue.IsError);
            bool canLaunch = _session != null && _isPlayingShown == false && hasErrors == false;

            _playButton.SetEnabled(canLaunch);
            _playFromStageButton.SetEnabled(canLaunch && HasSelectedStage);
            _goldField.SetEnabled(_isPlayingShown == false && HasSelectedStage);

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

            return string.Empty;
        }

        // При старте не с первого стейджа счёт волн в игре сдвинут — строка подсказывает и игровой номер.
        private static string DescribeLaunch(PlaytestLaunch launch, int stageNumber)
        {
            string text = $"Идёт плейтест: {launch.Level.name}";

            if (launch.StartGold.HasValue)
                text += $", золото на старте {launch.StartGold.Value}";

            if (stageNumber == PlaytestLogService.NOT_STARTED_STAGE_NUMBER)
                return text + " · подготовка";

            int stagesCount = launch.Level.Stages.Count;
            text += $" · стейдж {stageNumber} из {stagesCount}";

            if (launch.StartStageIndex == FIRST_STAGE_INDEX)
                return text;

            int gameWaveNumber = stageNumber - launch.StartStageIndex;

            return text + $" (в игре «Волна {gameWaveNumber}»)";
        }

        private void OnPlayClicked()
        {
            PlaytestLauncher.Launch(new PlaytestLaunch(_session.Config, FIRST_STAGE_INDEX, startGold: null));
        }

        private void OnPlayFromStageClicked()
        {
            int startGold = Mathf.Max(LevelConfigLimits.MIN_START_GOLD, _goldField.value);

            PlaytestLauncher.Launch(new PlaytestLaunch(_session.Config, _stageIndex, startGold));
        }
    }
}
