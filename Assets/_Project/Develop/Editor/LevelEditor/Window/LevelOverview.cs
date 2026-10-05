using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Итоги одного уровня для вкладки «Все уровни». Флаг несохранённого и счётчик правок запоминаются,
    // чтобы заметить, что уровень изменили или сохранили со стороны.
    public class LevelOverview
    {
        public LevelConfig Config { get; set; }
        public int? CampaignNumber { get; set; }
        public LevelStats Stats { get; set; }
        public IReadOnlyList<LevelIssue> Issues { get; set; }
        public bool IsDirty { get; set; }
        public int DirtyCount { get; set; }
    }
}
