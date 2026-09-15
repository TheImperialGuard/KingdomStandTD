using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        public event Action PauseClicked;

        [field: SerializeField] public IconTextView GoldWalletView { get; private set; }
        [field: SerializeField] public IconTextView PlayerHealthView { get; private set; }
        [field: SerializeField] public IconTextView StagesStatusView { get; private set; }
        [field: SerializeField] public Button PauseButton { get; private set; }

        private void OnEnable()
        {
            PauseButton.onClick.AddListener(OnPauseClicked);
        }

        private void OnDisable()
        {
            PauseButton.onClick.RemoveListener(OnPauseClicked);
        }

        private void OnPauseClicked() => PauseClicked?.Invoke();
    }
}
