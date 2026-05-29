using Assets._Project.Develop.Runtime.UI.Core.Views;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    public class LevelTileView : MonoBehaviour, IShowableView
    {
        public event Action Clicked;

        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _levelNumberText;
        [SerializeField] private Button _button;

        [SerializeField] private List<Image> _starFillers;

        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _completedColor;

        public Tween Show()
        {
            transform.DOKill();

            return transform
                .DOScale(1, 0.1f)
                .From(0)
                .SetUpdate(true)
                .Play();
        }

        public Tween Hide()
        {
            transform.DOKill();

            return DOTween.Sequence();
        }

        public void SetLevel(string level) => _levelNumberText.text = level;

        public void SetComplete() => _background.color = _completedColor;

        public void SetActive() => _background.color = _activeColor;

        public void SetStars(int activeCount)
        {
            if (activeCount > _starFillers.Count)
                throw new ArgumentOutOfRangeException(
                    $"Active stars count cannot exceed the number of star fillers:" +
                    $" {activeCount} given, max: {_starFillers.Count}");

            for (int i = 0; i < activeCount; i++)
            {
                _starFillers[i].gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        private void OnClick() => Clicked?.Invoke();
    }
}
