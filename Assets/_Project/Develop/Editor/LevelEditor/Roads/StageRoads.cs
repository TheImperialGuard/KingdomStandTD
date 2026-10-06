using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Assets._Project.Develop.Editor.LevelEditor.Roads
{
    // Дороги того, что сейчас открыто в Scene View: префаба уровня в Prefab Mode или сцен в Play Mode. Список
    // пересобирается после изменений иерархии, а не на каждой перерисовке.
    [InitializeOnLoad]
    public static class StageRoads
    {
        private static readonly List<RoadPath> _roads = new();

        private static bool _isOutdated = true;

        static StageRoads()
        {
            EditorApplication.hierarchyChanged += MarkOutdated;
            EditorApplication.playModeStateChanged += _ => MarkOutdated();
            PrefabStage.prefabStageOpened += _ => MarkOutdated();
            PrefabStage.prefabStageClosing += _ => MarkOutdated();
        }

        // Дороги по порядку номеров. Уничтоженная дорога — повод собрать список заново, даже без события.
        public static IReadOnlyList<RoadPath> Get()
        {
            if (_isOutdated || HasDestroyedRoads())
                Refresh();

            return _roads;
        }

        private static void MarkOutdated() => _isOutdated = true;

        private static bool HasDestroyedRoads()
        {
            foreach (RoadPath road in _roads)
            {
                if (road == null)
                    return true;
            }

            return false;
        }

        private static void Refresh()
        {
            RoadPath[] roads = StageUtility.GetCurrentStageHandle().FindComponentsOfType<RoadPath>();

            _roads.Clear();
            _roads.AddRange(roads.OrderBy(road => road.Number));
            _isOutdated = false;
        }
    }
}
