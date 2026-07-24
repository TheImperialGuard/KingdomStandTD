using Assets._Project.Develop.Runtime.UI.Core.Popups;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup
{
    public class StartStagesPopupView : PopupViewBase
    {
        public event Action StartButtonClicked;

        [SerializeField] private Button _startButton;
        [SerializeField] private Transform _infoContainer;

        [SerializeField] private TMP_Text _wavesEnemiesText;

        public void ShowInfo() => _infoContainer.gameObject.SetActive(true);

        public void HideInfo() => _infoContainer.gameObject.SetActive(false);

        public void SetWavesEnemiesText(string text) => _wavesEnemiesText.text = text;

        private void Awake()
        {
            HideInfo();
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        private void OnStartButtonClicked() => StartButtonClicked?.Invoke();
    }
}
