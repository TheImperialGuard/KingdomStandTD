using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup
{
    public class DefeatPopupView : ResultSheetPopupView
    {
        public event Action ContinueClicked;
        public event Action RestartClicked;

        [SerializeField] private TMP_Text _wave;

        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;

        public void SetWave(string wave) => _wave.text = wave;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _continueButton.onClick.AddListener(OnContinueButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            RemoveButtonsListeners();
        }

        private void OnDisable() => RemoveButtonsListeners();

        private void RemoveButtonsListeners()
        {
            _continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnContinueButtonClicked() => ContinueClicked?.Invoke();

        private void OnRestartButtonClicked() => RestartClicked?.Invoke();
    }
}
