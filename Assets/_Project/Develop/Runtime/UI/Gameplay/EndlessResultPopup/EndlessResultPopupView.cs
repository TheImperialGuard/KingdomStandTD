using System;
using Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.EndlessResultPopup
{
    // Грамота итога забега: под волной — строка рекорда или штамп «Новый рекорд!», который бьёт по листу после появления.
    public class EndlessResultPopupView : ResultSheetPopupView
    {
        private const float StampStartTime = 0.6f;
        private const float StampHitDuration = 0.245f;
        private const float StampSettleDuration = 0.105f;
        private const float StampStartScale = 2f;
        private const float StampHitScale = 0.94f;

        public event Action MenuClicked;
        public event Action RestartClicked;

        [SerializeField] private TMP_Text _wave;
        [SerializeField] private TMP_Text _record;

        [SerializeField] private CanvasGroup _stamp;
        [SerializeField] private TMP_Text _stampText;

        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _restartButton;

        private bool _isNewRecord;

        public void SetWave(string wave) => _wave.text = wave;

        public void SetRecord(string record)
        {
            _record.text = record;
            SetNewRecordShown(false);
        }

        public void SetNewRecord(string newRecord)
        {
            _stampText.text = newRecord;
            SetNewRecordShown(true);
        }

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

        // Удар штампа: масштаб 2 → 0,94 с проявлением и до 1, весь путь с разгоном. Поворот штампа задан в префабе.
        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            if (_isNewRecord == false)
                return;

            Transform stampTransform = _stamp.transform;

            stampTransform.localScale = Vector3.one * StampStartScale;
            _stamp.alpha = 0f;

            animation
                .Insert(StampStartTime, stampTransform.DOScale(StampHitScale, StampHitDuration).SetEase(Ease.InQuad))
                .Insert(StampStartTime, _stamp.DOFade(1f, StampHitDuration).SetEase(Ease.InQuad))
                .Insert(StampStartTime + StampHitDuration, stampTransform.DOScale(1f, StampSettleDuration).SetEase(Ease.InQuad));
        }

        private void OnDisable() => RemoveButtonsListeners();

        private void SetNewRecordShown(bool isNewRecord)
        {
            _isNewRecord = isNewRecord;

            _record.gameObject.SetActive(isNewRecord == false);
            _stamp.gameObject.SetActive(isNewRecord);
        }

        private void RemoveButtonsListeners()
        {
            _menuButton.onClick.RemoveListener(OnMenuButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnMenuButtonClicked() => MenuClicked?.Invoke();

        private void OnRestartButtonClicked() => RestartClicked?.Invoke();
    }
}
