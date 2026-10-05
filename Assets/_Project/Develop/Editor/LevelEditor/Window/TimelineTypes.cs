using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Модель таймлайна — что и где рисовать. Времена — в секундах от начала показанного отрезка:
    // от начала стейджа в режиме стейджа, от начала уровня без скипов в режиме уровня.
    public class TimelineModel
    {
        public TimelineMode Mode { get; set; }
        public float Duration { get; set; }
        public List<TimelineRow> Rows { get; } = new();
        public List<TimelineStage> Stages { get; } = new();
        public List<TimelineWave> Waves { get; } = new();
        public List<TimelineEnemy> Enemies { get; } = new();
    }

    // Строка «дорога × полоса». Волны строки, которые пересекаются по времени, лежат в разных подстроках.
    public class TimelineRow
    {
        public PathNumber PathNumber { get; set; }
        public PathLane Lane { get; set; }
        public string Label { get; set; }
        public string Tooltip { get; set; }
        public int SubRowsCount { get; set; } = 1;
    }

    // Отрезок стейджа; Skip — когда можно скипнуть, в последнем стейдже скипа нет.
    public class TimelineStage
    {
        public int StageIndex { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public float? Skip { get; set; }
        public string Tooltip { get; set; }
    }

    // Полоска волны от первого до последнего врага. OverflowStart — конец стейджа, если спавн идёт дольше.
    // TrackSlot — цвет подложки, если в волне один вид врагов, иначе подложка нейтральная.
    public class TimelineWave
    {
        public int StageIndex { get; set; }
        public int WaveIndex { get; set; }
        public int RowIndex { get; set; }
        public int SubRow { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public float? OverflowStart { get; set; }
        public int TrackSlot { get; set; } = TimelinePalette.NO_SLOT;
        public List<TimelineSpawn> Spawns { get; } = new();
        public string Tooltip { get; set; }
    }

    // Засечка выхода врага: время и номер врага в TimelineModel.Enemies, у группы без врага — LevelAddress.NO_INDEX.
    public readonly struct TimelineSpawn
    {
        public TimelineSpawn(float time, int enemyIndex)
        {
            Time = time;
            EnemyIndex = enemyIndex;
        }

        public float Time { get; }
        public int EnemyIndex { get; }
    }

    // Строка легенды: враг и его цвет. Строка есть у каждого врага — по ней врага подсвечивают на таймлайне;
    // врагам сверх палитры достаётся серый цвет «прочих».
    public class TimelineEnemy
    {
        public CharacterConfig Config { get; set; }
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public int Slot { get; set; }
    }
}
