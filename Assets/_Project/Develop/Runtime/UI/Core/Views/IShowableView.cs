using DG.Tweening;

namespace Assets._Project.Develop.Runtime.UI.Core.Views
{
    public interface IShowableView : IView
    {
        Tween Show();

        Tween Hide();
    }
}
