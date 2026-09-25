using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuInputArgs : IInputSceneArgs
    {
        public MainMenuInputArgs(string previousScene)
        {
            PreviousScene = previousScene;
        }

        public string PreviousScene { get; }
    }
}
