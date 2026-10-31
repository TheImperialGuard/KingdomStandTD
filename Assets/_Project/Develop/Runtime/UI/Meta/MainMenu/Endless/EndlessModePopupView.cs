using System;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Endless
{
    public class EndlessModePopupView : PopupViewBase
    {
        public event Action PlayClicked;
        public event Action BackClicked;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _backButton;

        public void SetTitle(string title) => _title.text = title;

        public void SetDescription(string description) => _description.text = description;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            RemoveButtonsListeners();
        }

        private void OnDisable() => RemoveButtonsListeners();

        private void RemoveButtonsListeners()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        private void OnPlayButtonClicked() => PlayClicked?.Invoke();

        private void OnBackButtonClicked() => BackClicked?.Invoke();
    }
}
