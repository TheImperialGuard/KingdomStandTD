using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuInputArgs : IInputSceneArgs
    {
        public MainMenuInputArgs(bool mustPlayActiveLevelAnimation)
        {
            MustPlayActiveLevelAnimation = mustPlayActiveLevelAnimation;
        }

        public bool MustPlayActiveLevelAnimation { get; }
    }
}
