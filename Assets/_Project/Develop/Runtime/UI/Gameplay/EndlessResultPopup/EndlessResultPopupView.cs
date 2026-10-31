using System;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.EndlessResultPopup
{
    public class EndlessResultPopupView : PopupViewBase
    {
        public event Action MenuClicked;
        public event Action RestartClicked;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _wave;
        [SerializeField] private TMP_Text _record;

        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _restartButton;

        public void SetTitle(string title) => _title.text = title;

        public void SetWave(string wave) => _wave.text = wave;

        public void SetRecord(string record) => _record.text = record;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _menuButton.onClick.AddListener(OnMenuButtonClicked);
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
            _menuButton.onClick.RemoveListener(OnMenuButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnMenuButtonClicked() => MenuClicked?.Invoke();

        private void OnRestartButtonClicked() => RestartClicked?.Invoke();
    }
}
