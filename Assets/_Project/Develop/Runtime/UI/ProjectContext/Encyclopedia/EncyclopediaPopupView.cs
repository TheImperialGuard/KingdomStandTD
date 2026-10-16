using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Книга энциклопедии (spec-encyclopedia.md): разворот с закладками разделов, номерами страниц и стрелками, обложка
    // переворачивается при появлении. Место и масштаб задаёт вызывающий: в меню — расчёт раскладки меню (Р-66).
    // Числа анимаций — в секундах и px макета.
    public class EncyclopediaPopupView : PopupViewBase
    {
        private const float MockupToReference = 1.5f;
        private const float RiseOffset = 24f;
        private const float FadeInDuration = 0.25f;
        private const float CoverFlipStart = 0.6f;
        private const float CoverFlipDuration = 0.3f;
        private const float UnfoldStart = 0.9f;
        private const float UnfoldDuration = 0.3f;
        private const float HideDuration = 0.2f;

        public event Action<EncyclopediaSections> SectionClicked;
        public event Action PreviousClicked;
        public event Action NextClicked;

        [SerializeField] private RectTransform _fit;
        [SerializeField] private RectTransform _book;
        [SerializeField] private CanvasGroup _bookGroup;
        [SerializeField] private RectTransform _cover;
        [SerializeField] private RectTransform _leftHalf;

        [SerializeField] private List<EncyclopediaBookmarkView> _bookmarks;
        [SerializeField] private EncyclopediaArrowView _previousArrow;
        [SerializeField] private EncyclopediaArrowView _nextArrow;
        [SerializeField] private TMP_Text _leftPageNumber;
        [SerializeField] private TMP_Text _rightPageNumber;

        [SerializeField] private EncyclopediaLeftPageView _leftPage;
        [SerializeField] private EncyclopediaEntityPageView _entityPage;
        [SerializeField] private EncyclopediaNotePageView _notePage;

        [SerializeField] private List<EncyclopediaSectionStyle> _sectionStyles;

        public EncyclopediaLeftPageView LeftPage => _leftPage;

        public EncyclopediaEntityPageView EntityPage => _entityPage;

        public EncyclopediaNotePageView NotePage => _notePage;

        private void OnEnable()
        {
            foreach (EncyclopediaBookmarkView bookmark in _bookmarks)
                bookmark.Clicked += OnBookmarkClicked;

            _previousArrow.Clicked += OnPreviousArrowClicked;
            _nextArrow.Clicked += OnNextArrowClicked;
        }

        private void OnDisable()
        {
            foreach (EncyclopediaBookmarkView bookmark in _bookmarks)
                bookmark.Clicked -= OnBookmarkClicked;

            _previousArrow.Clicked -= OnPreviousArrowClicked;
            _nextArrow.Clicked -= OnNextArrowClicked;
        }

        // Книга с закладками прижата правым краем к полю и масштабируется от него.
        public void SetLayout(float scale, float rightMargin)
        {
            _fit.localScale = new Vector3(scale, scale, 1f);
            _fit.anchoredPosition = new Vector2(-rightMargin, _fit.anchoredPosition.y);
        }

        public void SetSection(EncyclopediaSections section)
        {
            foreach (EncyclopediaBookmarkView bookmark in _bookmarks)
                bookmark.SetSelected(bookmark.Section == section);

            EncyclopediaSectionStyle style = GetStyle(section);
            bool isTutorial = section == EncyclopediaSections.Tutorial;

            _leftPage.SetPlaceholder(style.PlaceholderIcon, style.PlaceholderCaption);
            _entityPage.SetAccentColors(style.AccentColor, style.AccentDarkColor);
            _notePage.SetAccentColor(style.AccentColor);

            _entityPage.gameObject.SetActive(isTutorial == false);
            _notePage.gameObject.SetActive(isTutorial);
        }

        public void SetPageNumbers(int leftPageNumber, int rightPageNumber)
        {
            _leftPageNumber.text = leftPageNumber.ToString();
            _rightPageNumber.text = rightPageNumber.ToString();
        }

        public void SetArrows(bool canTurnBack, bool canTurnForward)
        {
            _previousArrow.SetInteractable(canTurnBack);
            _nextArrow.SetInteractable(canTurnForward);
        }

        // Появление: проявление с подъёмом, книга лежит закрытой, обложка переворачивается к корешку, затем
        // разворачивается левая половина.
        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            float riseOffset = RiseOffset * MockupToReference;

            _bookGroup.alpha = 0f;
            _book.anchoredPosition = Vector2.down * riseOffset;
            _cover.localScale = Vector3.one;
            _leftHalf.localScale = new Vector3(0f, 1f, 1f);

            animation
                .Insert(0f, _bookGroup.DOFade(1f, FadeInDuration).SetEase(Ease.OutQuad))
                .Insert(0f, _book.DOAnchorPosY(0f, FadeInDuration).SetEase(Ease.OutQuad))
                .Insert(CoverFlipStart, _cover.DOScaleX(0f, CoverFlipDuration).SetEase(Ease.InQuad))
                .Insert(UnfoldStart, _leftHalf.DOScaleX(1f, UnfoldDuration).SetEase(Ease.OutQuad));
        }

        // Общее скрытие попапов мгновенное: книга сама гаснет и опускается (Р-24).
        protected override void ModifyHideAnimation(Sequence animation)
        {
            base.ModifyHideAnimation(animation);

            float riseOffset = RiseOffset * MockupToReference;

            animation
                .Insert(0f, _bookGroup.DOFade(0f, HideDuration).SetEase(Ease.InQuad))
                .Insert(0f, _book.DOAnchorPosY(-riseOffset, HideDuration).SetEase(Ease.InQuad));
        }

        private EncyclopediaSectionStyle GetStyle(EncyclopediaSections section)
        {
            foreach (EncyclopediaSectionStyle style in _sectionStyles)
            {
                if (style.Section == section)
                    return style;
            }

            throw new ArgumentException($"Not found encyclopedia style for section: {section}");
        }

        private void OnBookmarkClicked(EncyclopediaSections section) => SectionClicked?.Invoke(section);

        private void OnPreviousArrowClicked() => PreviousClicked?.Invoke();

        private void OnNextArrowClicked() => NextClicked?.Invoke();
    }
}
