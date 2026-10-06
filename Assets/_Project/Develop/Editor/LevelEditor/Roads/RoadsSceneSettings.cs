using UnityEditor;

namespace Assets._Project.Develop.Editor.LevelEditor.Roads
{
    // Что рисовать в Scene View: дороги целиком и их части. Хранится в EditorPrefs — у каждого пользователя своё.
    public static class RoadsSceneSettings
    {
        private const string PREFS_PREFIX = "KingdomStandTD.LevelEditor.Roads.";
        private const string ROADS_KEY = PREFS_PREFIX + "RoadsShown";
        private const string LANES_KEY = PREFS_PREFIX + "LanesShown";
        private const string LABELS_KEY = PREFS_PREFIX + "LabelsShown";
        private const string WAYPOINT_NUMBERS_KEY = PREFS_PREFIX + "WaypointNumbersShown";

        private static bool _areRoadsShown = EditorPrefs.GetBool(ROADS_KEY, true);
        private static bool _areLanesShown = EditorPrefs.GetBool(LANES_KEY, true);
        private static bool _areLabelsShown = EditorPrefs.GetBool(LABELS_KEY, true);
        private static bool _areWaypointNumbersShown = EditorPrefs.GetBool(WAYPOINT_NUMBERS_KEY, false);

        // Главный переключатель: без него не рисуется ничего.
        public static bool AreRoadsShown
        {
            get => _areRoadsShown;
            set
            {
                _areRoadsShown = value;
                Save(ROADS_KEY, value);
            }
        }

        public static bool AreLanesShown
        {
            get => _areLanesShown;
            set
            {
                _areLanesShown = value;
                Save(LANES_KEY, value);
            }
        }

        public static bool AreLabelsShown
        {
            get => _areLabelsShown;
            set
            {
                _areLabelsShown = value;
                Save(LABELS_KEY, value);
            }
        }

        public static bool AreWaypointNumbersShown
        {
            get => _areWaypointNumbersShown;
            set
            {
                _areWaypointNumbersShown = value;
                Save(WAYPOINT_NUMBERS_KEY, value);
            }
        }

        private static void Save(string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
            SceneView.RepaintAll();
        }
    }
}
