using System;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Functional;
using Assets._Project.Develop.Runtime.UI.Settings;
using Assets._Project.Develop.Runtime.Utilities.Browser;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.PausePopup
{
    // Пауза боя — доска на цепях в масштабе меню по центру safe area (spec-pause.md, Р-06, Р-15): падает при открытии,
    // закрывается сразу. Раскладку окно считает само: перед показом и при смене размера экрана.
    public class PausePopupView : ChainBoardPopupView
    {
        public event Action ContinueClicked;
        public event Action ExitClicked;
        public event Action RestartClicked;

        [SerializeField] private RectTransform _fit;

        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private AudioSettingsView _audioSettingsView;

        private Canvas _canvas;

        public AudioSettingsView AudioSettingsView => _audioSettingsView;

        protected override bool RaisesOnHide => false;

        private Canvas RootCanvas => _canvas != null ? _canvas : _canvas = GetComponentInParent<Canvas>().rootCanvas;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            UpdateLayout();

            _continueButton.onClick.AddListener(OnContinueButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            RemoveButtonListeners();
        }

        private void OnDisable() => RemoveButtonListeners();

        // Корень растянут на слой попапов и меняет размер вместе с экраном, поэтому своего Update не нужно.
        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled)
                UpdateLayout();
        }

        private void UpdateLayout()
        {
            PausePopupLayout layout = PausePopupLayout.Calculate(
                new Vector2(Screen.width, Screen.height),
                Screen.safeArea,
                RootCanvas.scaleFactor,
                MenuCanvasScale.IsPhone(BrowserScreen.LogicalHeight));

            _fit.localScale = new Vector3(layout.Scale, layout.Scale, 1f);
            _fit.anchoredPosition = new Vector2(layout.CenterOffset, 0f);

            SetLayout(PausePopupLayout.PanelWidth, layout.PanelTop, 0f);
        }

        private void RemoveButtonListeners()
        {
            _continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnContinueButtonClicked() => ContinueClicked?.Invoke();

        private void OnExitButtonClicked() => ExitClicked?.Invoke();

        private void OnRestartButtonClicked() => RestartClicked?.Invoke();
    }
}
