using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    // Плитка уровня: пройден (звёзды), текущий (▶ и покачивающийся ▼ над плиткой), закрыт (замок), «В разработке».
    // Кромка при наведении — Sprite Swap кнопки; увеличение номера и нажатие — PointerFeedbackView.
    public class LevelTileView : MonoBehaviour, IView
    {
        public event Action Clicked;

        [SerializeField] private Button _button;
        [SerializeField] private PointerFeedbackView _pointerFeedback;
        [SerializeField] private Image _plate;
        [SerializeField] private VerticalLayoutGroup _face;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private GameObject _stars;
        [SerializeField] private List<Image> _starImages;
        [SerializeField] private GameObject _nextIcon;
        [SerializeField] private GameObject _lockIcon;
        [SerializeField] private GameObject _inDevelopmentPlate;
        [SerializeField] private TMP_Text _inDevelopmentText;
        [SerializeField] private RectTransform _currentPointer;

        [SerializeField] private Sprite _plateSprite;
        [SerializeField] private Sprite _plateHoveredSprite;
        [SerializeField] private Sprite _currentPlateSprite;
        [SerializeField] private Sprite _currentPlateHoveredSprite;
        [SerializeField] private Sprite _starOnSprite;
        [SerializeField] private Sprite _starOffSprite;

        [SerializeField] private Color _closedPlateColor = new(0.72f, 0.72f, 0.72f, 1f);
        [SerializeField] private Color _numberColor = Color.white;
        [SerializeField] private Color _closedNumberColor = Color.white;
        [SerializeField] private Material _numberMaterial;
        [SerializeField] private Material _closedNumberMaterial;

        [SerializeField, Min(1f)] private float _largeNumberSize = 48f;
        [SerializeField, Min(1f)] private float _smallNumberSize = 39f;
        [SerializeField, Min(0f)] private float _largeFaceSpacing = 9f;
        [SerializeField, Min(0f)] private float _smallFaceSpacing = 3f;
        [SerializeField, Min(1f)] private float _largeInDevelopmentTextSize = 19.5f;
        [SerializeField, Min(1f)] private float _smallInDevelopmentTextSize = 16.5f;

        [SerializeField] private float _pointerBobOffset = 7.5f;
        [SerializeField, Min(0.01f)] private float _pointerBobStepDuration = 0.4f;

        private Vector2 _pointerIdlePosition;
        private Sequence _pointerBob;

        private void Awake()
        {
            _pointerIdlePosition = _currentPointer.anchoredPosition;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnDestroy() => _pointerBob?.Kill();

        public void SetNumber(int levelNumber) => _numberText.text = levelNumber.ToString();

        // Крупная плитка — от 90 px макета: на ней номер и надпись «В разработке» крупнее, зазор под номером больше.
        public void SetLarge(bool isLarge)
        {
            _numberText.fontSize = isLarge ? _largeNumberSize : _smallNumberSize;
            _face.spacing = isLarge ? _largeFaceSpacing : _smallFaceSpacing;
            _inDevelopmentText.fontSize = isLarge ? _largeInDevelopmentTextSize : _smallInDevelopmentTextSize;
        }

        public void SetCompleted(LevelResults results)
        {
            ApplyState(isPlayable: true, isCurrent: false);
            SetStars(GetStarsCount(results));
            _stars.SetActive(true);
        }

        public void SetCurrent() => ApplyState(isPlayable: true, isCurrent: true);

        public void SetClosed()
        {
            ApplyState(isPlayable: false, isCurrent: false);
            _lockIcon.SetActive(true);
        }

        public void SetInDevelopment()
        {
            ApplyState(isPlayable: false, isCurrent: false);
            _numberText.gameObject.SetActive(false);
            _inDevelopmentPlate.SetActive(true);
        }

        private void ApplyState(bool isPlayable, bool isCurrent)
        {
            _button.interactable = isPlayable;

            _plate.sprite = isCurrent ? _currentPlateSprite : _plateSprite;
            _plate.color = isPlayable ? Color.white : _closedPlateColor;

            Sprite hoveredSprite = isCurrent ? _currentPlateHoveredSprite : _plateHoveredSprite;

            _button.spriteState = new SpriteState
            {
                highlightedSprite = hoveredSprite,
                pressedSprite = hoveredSprite,
            };

            _numberText.gameObject.SetActive(true);
            _numberText.color = isPlayable ? _numberColor : _closedNumberColor;
            _numberText.fontSharedMaterial = isPlayable ? _numberMaterial : _closedNumberMaterial;

            _stars.SetActive(false);
            _nextIcon.SetActive(isCurrent);
            _lockIcon.SetActive(false);
            _inDevelopmentPlate.SetActive(false);
            _currentPointer.gameObject.SetActive(isCurrent);

            if (isCurrent)
                StartPointerBob();
            else
                _pointerBob?.Kill();

            _pointerFeedback.Refresh();
        }

        private void SetStars(int count)
        {
            for (int i = 0; i < _starImages.Count; i++)
                _starImages[i].sprite = i < count ? _starOnSprite : _starOffSprite;
        }

        private int GetStarsCount(LevelResults results)
        {
            switch (results)
            {
                case LevelResults.Perfect:
                    return 3;

                case LevelResults.Average:
                    return 2;

                case LevelResults.Bad:
                    return 1;

                case LevelResults.Defeat:
                    return 0;

                default:
                    throw new ArgumentException($"{nameof(results)} result not supported in LevelTileView");
            }
        }

        // ▼ переступает ступенькой вниз и обратно, как steps(1, end) в макете.
        private void StartPointerBob()
        {
            _pointerBob?.Kill();
            SetPointerOffset(0f);

            _pointerBob = DOTween.Sequence()
                .AppendInterval(_pointerBobStepDuration)
                .AppendCallback(() => SetPointerOffset(_pointerBobOffset))
                .AppendInterval(_pointerBobStepDuration)
                .AppendCallback(() => SetPointerOffset(0f))
                .SetLoops(-1)
                .SetUpdate(true);
        }

        private void SetPointerOffset(float offset)
            => _currentPointer.anchoredPosition = _pointerIdlePosition + Vector2.down * offset;

        private void OnClick() => Clicked?.Invoke();
    }
}
