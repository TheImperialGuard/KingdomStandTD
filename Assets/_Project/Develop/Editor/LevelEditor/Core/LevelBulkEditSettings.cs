namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Параметры массовой операции: область — стейджи с первого по последний (индексы с нуля, включительно),
    // количество врагов и интервал спавна — в процентах, старт волн, время стейджа и скип — сдвигом в секундах.
    // 100 % и 0 с значат «не менять».
    public class LevelBulkEditSettings
    {
        public const int UNCHANGED_PERCENT = 100;

        public int FirstStageIndex { get; set; }
        public int LastStageIndex { get; set; }
        public int CountPercent { get; set; } = UNCHANGED_PERCENT;
        public int DelayPercent { get; set; } = UNCHANGED_PERCENT;
        public float StartShift { get; set; }
        public float StageTimeShift { get; set; }
        public float SkipShift { get; set; }
    }
}
