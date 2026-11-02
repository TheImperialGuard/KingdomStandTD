using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    // Аргументы сцены Gameplay: кампания и номер её уровня или бесконечный уровень. «Заново» передаёт их копию с
    // признаком перезапуска — и перезапускает тот уровень, что шёл.
    public class GameplayInputArgs : IInputSceneArgs
    {
        private const int EndlessLevelNumber = 0;

        private GameplayInputArgs(GameplayLevelKinds levelKind, int levelNumber, bool isRestart)
        {
            LevelKind = levelKind;
            LevelNumber = levelNumber;
            IsRestart = isRestart;
        }

        public GameplayLevelKinds LevelKind { get; }

        // Номер уровня кампании; у бесконечного уровня номера нет
        public int LevelNumber { get; }

        // Уровень запущен заново из боя («Заново»), а не из меню
        public bool IsRestart { get; }

        public bool IsEndless => LevelKind == GameplayLevelKinds.Endless;

        public static GameplayInputArgs ForCampaign(int levelNumber) => new(GameplayLevelKinds.Campaign, levelNumber, false);

        public static GameplayInputArgs ForEndless() => new(GameplayLevelKinds.Endless, EndlessLevelNumber, false);

        public GameplayInputArgs AsRestart() => new(LevelKind, LevelNumber, true);
    }
}
