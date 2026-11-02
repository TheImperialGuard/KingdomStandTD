using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup
{
    // Грамота победы: пустые слоты звёзд видны всегда, заработанные золотые падают в них по очереди «из экрана в лист»
    // с отскоком. Выход — только печатью.
    public class WinPopupView : ResultSheetPopupView
    {
        private const float StarsStartTime = 0.5f;
        private const float StarsInterval = 0.4f;
        private const float StarFallDuration = 0.39f;
        private const float StarBounceDuration = 0.1f;
        private const float StarSettleDuration = 0.11f;
        private const float StarStartScale = 2f;
        private const float StarBounceScale = 1.04f;

        public event Action ContinueClicked;

        [SerializeField] private List<Image> _earnedStars;
        [SerializeField] private TMP_Text _lives;
        [SerializeField] private Button _sealButton;

        private LevelResults _levelResults;

        public void SetResults(LevelResults results) => _levelResults = results;

        public void SetLives(string lives) => _lives.text = lives;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _sealButton.onClick.AddListener(OnSealButtonClicked);
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _sealButton.onClick.RemoveListener(OnSealButtonClicked);
        }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            int starsCount = GetStarsCount();

            for (int i = 0; i < _earnedStars.Count; i++)
            {
                Image star = _earnedStars[i];
                bool isEarned = i < starsCount;

                star.gameObject.SetActive(isEarned);

                if (isEarned)
                    InsertStarFall(animation, star, StarsStartTime + StarsInterval * i);
            }
        }

        private void OnDisable() => _sealButton.onClick.RemoveListener(OnSealButtonClicked);

        private int GetStarsCount()
        {
            return _levelResults switch
            {
                LevelResults.Perfect => 3,
                LevelResults.Average => 2,
                LevelResults.Bad => 1,
                LevelResults.Defeat => throw new InvalidOperationException("Win popup opened on defeat level results"),
                _ => throw new ArgumentException($"{_levelResults} not supported in {this}"),
            };
        }

        // Падение: масштаб 2 → 1 с проявлением, отскок до 1,04 и возврат к 1 — кривые по ключам кадров макета.
        private void InsertStarFall(Sequence animation, Image star, float startTime)
        {
            Transform starTransform = star.transform;
            float bounceTime = startTime + StarFallDuration;
            float settleTime = bounceTime + StarBounceDuration;

            starTransform.localScale = Vector3.one * StarStartScale;
            star.color = new Color(star.color.r, star.color.g, star.color.b, 0f);

            animation
                .Insert(startTime, starTransform.DOScale(1f, StarFallDuration).SetEase(Ease.InQuad))
                .Insert(startTime, star.DOFade(1f, StarFallDuration).SetEase(Ease.InQuad))
                .Insert(bounceTime, starTransform.DOScale(StarBounceScale, StarBounceDuration).SetEase(Ease.OutQuad))
                .Insert(settleTime, starTransform.DOScale(1f, StarSettleDuration).SetEase(Ease.InOutQuad));
        }

        private void OnSealButtonClicked() => ContinueClicked?.Invoke();
    }
}
