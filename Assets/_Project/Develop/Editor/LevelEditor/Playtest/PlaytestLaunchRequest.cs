using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Запрос на плейтест, который переживает перезагрузку скриптов при входе в Play Mode: окно записывает его перед
    // запуском, а после перезагрузки его читает PlaytestLauncher. SessionState живёт до закрытия редактора.
    public static class PlaytestLaunchRequest
    {
        private const string KEY_PREFIX = "KingdomStandTD.Playtest.";
        private const string LEVEL_GUID_KEY = KEY_PREFIX + "LevelGuid";
        private const string START_STAGE_KEY = KEY_PREFIX + "StartStage";
        private const string START_GOLD_KEY = KEY_PREFIX + "StartGold";

        private const int NO_START_GOLD = -1;

        public static bool IsPending => string.IsNullOrEmpty(SessionState.GetString(LEVEL_GUID_KEY, string.Empty)) == false;

        public static void Write(PlaytestLaunch launch)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(launch.Level));

            SessionState.SetString(LEVEL_GUID_KEY, guid);
            SessionState.SetInt(START_STAGE_KEY, launch.StartStageIndex);
            SessionState.SetInt(START_GOLD_KEY, launch.StartGold ?? NO_START_GOLD);
        }

        public static bool TryRead(out PlaytestLaunch launch)
        {
            launch = null;

            string guid = SessionState.GetString(LEVEL_GUID_KEY, string.Empty);
            LevelConfig level = AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetDatabase.GUIDToAssetPath(guid));

            if (level == null)
                return false;

            int startStageIndex = SessionState.GetInt(START_STAGE_KEY, 0);
            int savedGold = SessionState.GetInt(START_GOLD_KEY, NO_START_GOLD);
            int? startGold = savedGold == NO_START_GOLD ? (int?)null : savedGold;

            launch = new PlaytestLaunch(level, startStageIndex, startGold);

            return true;
        }

        public static void Clear()
        {
            SessionState.EraseString(LEVEL_GUID_KEY);
            SessionState.EraseInt(START_STAGE_KEY);
            SessionState.EraseInt(START_GOLD_KEY);
        }
    }
}
