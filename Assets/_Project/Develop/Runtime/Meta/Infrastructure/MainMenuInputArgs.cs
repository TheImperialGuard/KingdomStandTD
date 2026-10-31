using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    // Аргументы сцены MainMenu. После боя меню открывает доску «Играть» на шаге, откуда в бой пришли: на уровнях кампании
    // или, после бесконечного забега, на режимах игры.
    public class MainMenuInputArgs : IInputSceneArgs
    {
        public MainMenuInputArgs(string previousScene) : this(previousScene, PlayBoardSteps.Campaign)
        {
        }

        private MainMenuInputArgs(string previousScene, PlayBoardSteps playBoardStep)
        {
            PreviousScene = previousScene;
            PlayBoardStep = playBoardStep;
        }

        public string PreviousScene { get; }

        // Шаг доски «Играть», на котором она открывается после боя
        public PlayBoardSteps PlayBoardStep { get; }

        public static MainMenuInputArgs FromGameplay(GameplayInputArgs gameplayInputArgs)
        {
            PlayBoardSteps playBoardStep = gameplayInputArgs.IsEndless ? PlayBoardSteps.Modes : PlayBoardSteps.Campaign;

            return new MainMenuInputArgs(Scenes.Gameplay, playBoardStep);
        }
    }
}
