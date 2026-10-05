namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Замечание проверки уровня. Индексы стейджа, волны и группы — с нуля; LevelAddress.NO_INDEX — не к этому месту.
    public class LevelIssue
    {
        public LevelIssue(LevelIssueSeverity severity, string message, int stageIndex, int waveIndex, int groupIndex)
        {
            Severity = severity;
            Message = message;
            StageIndex = stageIndex;
            WaveIndex = waveIndex;
            GroupIndex = groupIndex;
        }

        public LevelIssueSeverity Severity { get; }
        public string Message { get; }
        public int StageIndex { get; }
        public int WaveIndex { get; }
        public int GroupIndex { get; }

        public bool IsError => Severity == LevelIssueSeverity.Error;

        public string Address => LevelAddress.Format(StageIndex, WaveIndex, GroupIndex);

        public static LevelIssue Error(
            string message,
            int stageIndex = LevelAddress.NO_INDEX,
            int waveIndex = LevelAddress.NO_INDEX,
            int groupIndex = LevelAddress.NO_INDEX)
        {
            return new LevelIssue(LevelIssueSeverity.Error, message, stageIndex, waveIndex, groupIndex);
        }

        public static LevelIssue Warning(
            string message,
            int stageIndex = LevelAddress.NO_INDEX,
            int waveIndex = LevelAddress.NO_INDEX,
            int groupIndex = LevelAddress.NO_INDEX)
        {
            return new LevelIssue(LevelIssueSeverity.Warning, message, stageIndex, waveIndex, groupIndex);
        }

        public override string ToString()
        {
            string address = Address;

            if (address.Length == 0)
                return Message;

            return $"{address}: {Message}";
        }
    }
}
