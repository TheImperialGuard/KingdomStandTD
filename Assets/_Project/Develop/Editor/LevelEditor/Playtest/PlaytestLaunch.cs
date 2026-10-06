using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Что запустить в плейтесте: уровень, стейдж, с которого начать, и золото на старте
    // (null — стартовое золото уровня, как в игре).
    public class PlaytestLaunch
    {
        public PlaytestLaunch(LevelConfig level, int startStageIndex, int? startGold)
        {
            Level = level;
            StartStageIndex = startStageIndex;
            StartGold = startGold;
        }

        public LevelConfig Level { get; }
        public int StartStageIndex { get; }
        public int? StartGold { get; }
    }
}
