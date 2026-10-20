using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Functional;
using Assets._Project.Develop.Runtime.UI.SingleAnimations;
using Assets._Project.Develop.Runtime.Utilities.Browser;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup
{
    // Кнопка волны: медальон с подсказкой под ним. На телефоне медальон крупнее (_xl, Р-07), подсказка опускается вслед
    // за его низом. Появление — twpop макета, пульсация (если есть) включается после него.
    public class StartStagesPopupView : PopupViewBase
    {
        private const float AppearDuration = 0.18f;
        private const float AppearStartScale = 0.6f;
        private const float TipGap = 27f;

        public event Action StartButtonClicked;

        [SerializeField] private MedallionButtonView _startButton;
        [SerializeField] private RectTransform _startButtonRect;
        [SerializeField] private CanvasGroup _appearGroup;
        [SerializeField] private BounceTween _idleAnimation;

        [SerializeField] private ParchmentTipView _tip;
        [SerializeField] private RectTransform _tipRect;
        [SerializeField] private TMP_Text _wavesEnemiesText;

        [SerializeField] private MedallionFaces _faces;
        [SerializeField] private MedallionFaces _phoneFaces;
        [SerializeField] private float _buttonSize = 120f;
        [SerializeField] private float _phoneButtonSize = 144f;

        public void ShowInfo()
        {
            _startButton.SetState(MedallionStates.Armed);
            _tip.Show();
        }

        public void HideInfo()
        {
            _startButton.SetState(MedallionStates.Idle);
            _tip.Hide();
        }

        public void SetWavesEnemiesText(string text)
        {
            _wavesEnemiesText.text = text;
            _tip.Refresh();
        }

        protected void RefreshTip() => _tip.Refresh();

        protected virtual void ApplyScreenMode(bool isPhone)
        {
            float buttonSize = isPhone ? _phoneButtonSize : _buttonSize;

            _startButton.SetFaces(isPhone ? _phoneFaces : _faces);
            _startButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
            _startButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);

            _tipRect.anchoredPosition = Vector2.down * (buttonSize / 2f + TipGap);
        }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            _appearGroup.alpha = 0f;
            _appearGroup.transform.localScale = Vector3.one * AppearStartScale;

            animation
                .Insert(0f, _appearGroup.DOFade(1f, AppearDuration).SetEase(Ease.OutQuad))
                .Insert(0f, _appearGroup.transform.DOScale(1f, AppearDuration).SetEase(Ease.OutQuad));
        }

        protected override void OnPostShow()
        {
            if (_idleAnimation != null)
                _idleAnimation.enabled = true;
        }

        private void Awake()
        {
            HideInfo();
            UpdateScreenMode();
        }

        private void OnEnable()
        {
            _startButton.Clicked += OnStartButtonClicked;
        }

        private void OnDisable()
        {
            _startButton.Clicked -= OnStartButtonClicked;
        }

        // Корень растянут на весь слой попапов: размер окна меняет и его.
        private void OnRectTransformDimensionsChange() => UpdateScreenMode();

        private void UpdateScreenMode() => ApplyScreenMode(MenuCanvasScale.IsPhone(BrowserScreen.LogicalHeight));

        private void OnStartButtonClicked() => StartButtonClicked?.Invoke();
    }
}
