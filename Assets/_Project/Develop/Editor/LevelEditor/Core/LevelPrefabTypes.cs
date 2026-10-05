using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    public enum LevelPrefabStatus
    {
        Found,
        PathMissing,
        PrefabMissing,
        LevelComponentMissing,
    }

    // Что редактор уровней знает о префабе уровня.
    public class LevelPrefabInfo
    {
        public LevelPrefabStatus Status { get; set; }
        public string AssetPath { get; set; }
        public int TowerSlotsCount { get; set; }
        public int MissingTowerSlotsCount { get; set; }
        public int MissingRoadsCount { get; set; }
        public List<LevelRoad> Roads { get; set; } = new();
    }

    // Дорога префаба. Длины по полосам есть, только если путь целый: от двух вейпоинтов и без пустых.
    public class LevelRoad
    {
        public PathNumber Number { get; set; }
        public string Name { get; set; }
        public int WaypointsCount { get; set; }
        public int MissingWaypointsCount { get; set; }
        public bool HasValidPath { get; set; }
        public Dictionary<PathLane, float> Lengths { get; set; } = new();
    }
}
