namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Итог забега бесконечного режима для окна итога: достигнутая волна и рекорд после этого забега.
    public readonly struct EndlessRunResult
    {
        public EndlessRunResult(int reachedWave, int bestWave, bool isNewRecord)
        {
            ReachedWave = reachedWave;
            BestWave = bestWave;
            IsNewRecord = isNewRecord;
        }

        public int ReachedWave { get; }

        public int BestWave { get; }

        public bool IsNewRecord { get; }
    }
}
