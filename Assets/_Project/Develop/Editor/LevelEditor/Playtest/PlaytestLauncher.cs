using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Запуск плейтеста из окна редактора уровней. Окно записывает запрос и входит в Play Mode; при входе скрипты
    // перезагружаются, и после перезагрузки этот класс открывает по запросу сессию и подключает её к игре через
    // PlaytestHooks. На выходе из Play Mode идущий прогон дописывается в лог, сессия закрывается, а запрос стирается.
    [InitializeOnLoad]
    public static class PlaytestLauncher
    {
        private static PlaytestSession _session;

        static PlaytestLauncher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (PlaytestLaunchRequest.IsPending == false)
                return;

            // Запрос без входа в Play Mode остался от неудачного запуска — например, при ошибках компиляции.
            if (EditorApplication.isPlayingOrWillChangePlaymode == false)
            {
                PlaytestLaunchRequest.Clear();
                return;
            }

            StartSession();
        }

        // Что играется в плейтесте сейчас; null, если плейтеста нет.
        public static PlaytestLaunch RunningLaunch => _session?.Launch;

        // Настоящий номер идущего стейджа плейтеста.
        public static int RunningStageNumber => _session?.CurrentStageNumber ?? PlaytestLogService.NOT_STARTED_STAGE_NUMBER;

        public static void Launch(PlaytestLaunch launch)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogWarning("Плейтест не запущен: сначала исправьте ошибки компиляции");
                return;
            }

            PlaytestLaunchRequest.Write(launch);
            EditorApplication.EnterPlaymode();
        }

        private static void StartSession()
        {
            if (PlaytestLaunchRequest.TryRead(out PlaytestLaunch launch) == false)
            {
                PlaytestLaunchRequest.Clear();
                Debug.LogError("Плейтест не запущен: уровень из запроса не найден");
                return;
            }

            _session = new PlaytestSession(launch, GetCampaignNumber(launch.Level));
            _session.Ended += EndSession;

            PlaytestHooks.Connect(_session);
        }

        private static void EndSession()
        {
            PlaytestLaunchRequest.Clear();

            if (_session == null)
                return;

            PlaytestHooks.Disconnect(_session);

            _session.Ended -= EndSession;
            _session.Dispose();
            _session = null;
        }

        private static int? GetCampaignNumber(LevelConfig level)
            => LevelEditorAssets.GetCampaignNumber(LevelEditorAssets.FindLevelsList(), level);

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    _session?.CompleteRun();
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    EndSession();
                    break;
            }
        }
    }
}
