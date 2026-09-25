using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        public event Action PauseClicked;
        public event Action AdvertRewardClicked;

        [field: SerializeField] public IconTextView GoldWalletView { get; private set; }
        [field: SerializeField] public IconTextView PlayerHealthView { get; private set; }
        [field: SerializeField] public IconTextView StagesStatusView { get; private set; }
        [field: SerializeField] public Button PauseButton { get; private set; }
        [field: SerializeField] public Button AdvertRewardButton { get; private set; }

        [SerializeField] private RectTransform _advertRewardTip;
        [SerializeField] private TMP_Text _advertRewardTipText;

        [SerializeField] private List<RectTransform> _tips;

        public void SetAdvertRewardTipText(string goldAmount)
        {
            _advertRewardTipText.text = $"<b>+{goldAmount}</b>  монет за просмотр рекламы!";
        }

        public void HideTips()
        {
            for (int i = 0; i < _tips.Count; i++)
            {
                _tips[i].gameObject.SetActive(false);
            }
        }

        public void HideAdvertRewardButton() => AdvertRewardButton.gameObject.SetActive(false);
        public void ShowAdvertRewardButton() => AdvertRewardButton.gameObject.SetActive(true);

        public void HideAdvertRewardTip() => _advertRewardTip.gameObject.SetActive(false);

        private void OnEnable()
        {
            PauseButton.onClick.AddListener(OnPauseClicked);
            AdvertRewardButton.onClick.AddListener(OnAdvertRewardButton);
        }

        private void OnDisable()
        {
            PauseButton.onClick.RemoveListener(OnPauseClicked);
            AdvertRewardButton.onClick.RemoveListener(OnAdvertRewardButton);
        }

        private void OnPauseClicked() => PauseClicked?.Invoke();

        private void OnAdvertRewardButton() => AdvertRewardClicked?.Invoke();
    }
}
