using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;

namespace Assets._Project.Develop.Runtime.Meta.Features.Endless
{
    // Рекорд бесконечного режима — лучшая волна, до которой дошёл забег. 0 — рекорда ещё нет: так читаются и сохранения,
    // сделанные до появления рекорда.
    public class EndlessRecordService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private const int NoRecord = 0;

        private int _bestWave = NoRecord;

        public EndlessRecordService(PlayerDataProvider playerDataProvider)
        {
            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public int BestWave => _bestWave;

        public bool HasRecord => _bestWave > NoRecord;

        // Рекорд обновляется, только если забег дошёл дальше прежнего
        public bool TryUpdate(int reachedWave)
        {
            if (reachedWave <= _bestWave)
                return false;

            _bestWave = reachedWave;

            return true;
        }

        public void ReadFrom(PlayerData data) => _bestWave = data.EndlessBestWave;

        public void WriteTo(PlayerData data) => data.EndlessBestWave = _bestWave;
    }
}
