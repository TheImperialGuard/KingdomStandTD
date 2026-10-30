using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    // Аргументы сцены Gameplay: кампания и номер её уровня или бесконечный уровень. «Заново» передаёт их же — и
    // перезапускает тот уровень, что шёл.
    public class GameplayInputArgs : IInputSceneArgs
    {
        private const int EndlessLevelNumber = 0;

        private GameplayInputArgs(GameplayLevelKinds levelKind, int levelNumber)
        {
            LevelKind = levelKind;
            LevelNumber = levelNumber;
        }

        public GameplayLevelKinds LevelKind { get; }

        // Номер уровня кампании; у бесконечного уровня номера нет
        public int LevelNumber { get; }

        public bool IsEndless => LevelKind == GameplayLevelKinds.Endless;

        public static GameplayInputArgs ForCampaign(int levelNumber) => new(GameplayLevelKinds.Campaign, levelNumber);

        public static GameplayInputArgs ForEndless() => new(GameplayLevelKinds.Endless, EndlessLevelNumber);
    }
}
