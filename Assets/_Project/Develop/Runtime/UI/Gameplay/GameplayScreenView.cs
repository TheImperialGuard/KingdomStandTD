using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public IconTextView GoldWalletView { get; private set; }
        [field: SerializeField] public IconTextView PlayerHealthView { get; private set; }
        [field: SerializeField] public IconTextView StagesStatusView { get; private set; }
    }
}
