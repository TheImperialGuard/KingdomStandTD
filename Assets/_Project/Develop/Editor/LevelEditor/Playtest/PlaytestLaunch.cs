using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.GameMode;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Что запустить в плейтесте: уровень, стейдж, с которого начать, и золото на старте
    // (null — стартовое золото уровня, как в игре). У бесконечного уровня своих стейджей нет: стартовый индекс — это
    // волна режима с нуля.
    public class PlaytestLaunch
    {
        private const int FirstWaveNumber = 1;

        public PlaytestLaunch(LevelConfig level, int startStageIndex, int? startGold)
        {
            Level = level;
            StartStageIndex = startStageIndex;
            StartGold = startGold;
        }

        public LevelConfig Level { get; }
        public int StartStageIndex { get; }
        public int? StartGold { get; }

        public bool IsEndless => Level.GameMode == GameModes.Infinity;

        // Стартовая волна бесконечного уровня
        public int StartWaveNumber => StartStageIndex + FirstWaveNumber;
    }
}
