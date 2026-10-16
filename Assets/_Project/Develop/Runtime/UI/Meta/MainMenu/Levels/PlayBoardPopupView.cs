using System;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    // Доска «Играть» на цепях: шаги «Режим игры» и «Кампания» сменяют содержимое одной доски (Р-11). Ширину и верх задаёт
    // общий расчёт раскладки, сетку уровней и прокрутку доска считает сама по формулам макета (spec-menu.md → «Уровни»).
    // Числа — в px макета.
    public class PlayBoardPopupView : PopupViewBase
    {
        private const int Columns = 5;
        private const int VisibleRows = 2;
        private const float MaxTileSize = 104f;
        private const float LargeTileMinSize = 90f;
        private const float TileGap = 14f;
        private const float BoardInset = 60f;
        private const float ListInset = 32f;
        private const float ScrollViewOverhang = 8f;
        private const float ScrollViewRightMargin = 16f;
        private const float ScrollGridSidePadding = 8f;
        private const float ScrollbarSpritePixel = 4f;
        private const float ChainOverlap = 20f;
        private const float DropHeight = 560f;
        private const float DropOvershoot = 12f;
        private const float DropDuration = 0.5f;
        private const float DropOvershootTime = 0.72f;
        private const float RaiseDuration = 0.2f;

        public event Action CampaignClicked;
        public event Action BackClicked;

        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _swing;
        [SerializeField] private RectTransform _board;
        [SerializeField] private RectTransform _leftChain;
        [SerializeField] private RectTransform _rightChain;

        [SerializeField] private GameObject _modesStep;
        [SerializeField] private GameObject _campaignStep;
        [SerializeField] private PlayModeCardView _campaignCard;
        [SerializeField] private PlayModeCardView _endlessCard;
        [SerializeField] private TMP_Text _campaignSubtitle;
        [SerializeField] private Button _backButton;

        [SerializeField] private LayoutElement _list;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectMask2D _viewportMask;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private RectTransform _scrollbar;
        [SerializeField] private LevelTilesListView _levelTilesListView;

        private bool _hasLayout;
        private float _panelWidth;
        private bool _isScrollable;
        private float _tileSize;
        private float _contentHeight;
        private float _viewHeight;
        private int _scrollTargetIndex;

        public LevelTilesListView LevelTilesListView => _levelTilesListView;

        private void OnEnable()
        {
            _campaignCard.Clicked += OnCampaignCardClicked;
            _campaignCard.HoverStarted += OnCardHoverStarted;
            _campaignCard.HoverEnded += OnCardHoverEnded;
            _endlessCard.HoverStarted += OnCardHoverStarted;
            _endlessCard.HoverEnded += OnCardHoverEnded;
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnDisable()
        {
            _campaignCard.Clicked -= OnCampaignCardClicked;
            _campaignCard.HoverStarted -= OnCardHoverStarted;
            _campaignCard.HoverEnded -= OnCardHoverEnded;
            _endlessCard.HoverStarted -= OnCardHoverStarted;
            _endlessCard.HoverEnded -= OnCardHoverEnded;
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        public void SetCampaignSubtitle(string subtitle) => _campaignSubtitle.text = subtitle;

        public void ShowStep(PlayBoardSteps step)
        {
            _modesStep.SetActive(step == PlayBoardSteps.Modes);
            _campaignStep.SetActive(step == PlayBoardSteps.Campaign);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_board);
        }

        public void SetLayout(float panelWidth, float panelTop, float panelRightMargin)
        {
            _hasLayout = true;
            _panelWidth = panelWidth;

            _panel.anchoredPosition = new Vector2(-panelRightMargin, 0f);
            _panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
            _board.anchoredPosition = new Vector2(0f, -panelTop);

            float chainHeight = panelTop + ChainOverlap * MainMenuLayout.MockupToReference;
            _leftChain.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chainHeight);
            _rightChain.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chainHeight);

            RefreshList();
        }

        // Сетка, окно прокрутки и полоса — по числу плиток и ширине доски. Зовётся после добавления плиток и при смене
        // раскладки.
        public void RefreshList()
        {
            if (_hasLayout == false)
                return;

            float scale = MainMenuLayout.MockupToReference;
            float contentWidth = _panelWidth / scale - 2f * BoardInset;
            int rows = Mathf.CeilToInt(_levelTilesListView.Elements.Count / (float)Columns);

            _tileSize = Mathf.Min(MaxTileSize, Mathf.Floor((contentWidth - (Columns - 1) * TileGap) / Columns));
            _isScrollable = rows > VisibleRows;

            float viewOverhang = _isScrollable ? ScrollViewOverhang : 0f;
            float viewRightMargin = _isScrollable ? ScrollViewRightMargin : 0f;
            float gridSidePadding = _isScrollable ? ScrollGridSidePadding : 0f;
            float listInset = _isScrollable ? ListInset : 0f;

            float gridWidth = contentWidth + viewOverhang - viewRightMargin - 2f * gridSidePadding;
            float cellWidth = (gridWidth - (Columns - 1) * TileGap) / Columns;
            float rowsHeight = GetRowsHeight(rows);

            _contentHeight = rowsHeight + 2f * listInset;
            _viewHeight = _isScrollable ? ListInset + GetRowsHeight(VisibleRows) : rowsHeight;

            _grid.cellSize = new Vector2(cellWidth, _tileSize) * scale;
            _grid.spacing = Vector2.one * TileGap * scale;

            int sidePadding = Mathf.RoundToInt(gridSidePadding * scale);
            int insetPadding = Mathf.RoundToInt(listInset * scale);
            _grid.padding = new RectOffset(sidePadding, sidePadding, insetPadding, insetPadding);

            _scrollRect.viewport.offsetMin = new Vector2(-viewOverhang * scale, 0f);
            _scrollRect.viewport.offsetMax = new Vector2(-viewRightMargin * scale, 0f);
            _scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _contentHeight * scale);
            _list.preferredHeight = _viewHeight * scale;

            // Без прокрутки ▼ над первым рядом выходит за сетку, как в макете: обрезка и прокрутка выключены.
            _viewportMask.enabled = _isScrollable;
            _scrollRect.enabled = _isScrollable;
            _scrollRect.StopMovement();
            _scrollRect.content.anchoredPosition = Vector2.zero;

            _scrollbar.gameObject.SetActive(_isScrollable);
            float trackHeight = Mathf.Floor(_viewHeight / ScrollbarSpritePixel) * ScrollbarSpritePixel;
            _scrollbar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, trackHeight * scale);

            bool isLarge = _tileSize >= LargeTileMinSize;

            foreach (LevelTileView tile in _levelTilesListView.Elements)
                tile.SetLarge(isLarge);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_board);
            ApplyScroll();
        }

        // Ряд плитки встаёт верхним видимым; в конце списка — насколько позволяет прокрутка.
        public void ScrollToTile(int tileIndex)
        {
            _scrollTargetIndex = tileIndex;
            ApplyScroll();
        }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            float scale = MainMenuLayout.MockupToReference;

            // Падение kbdrop: сверху с перелётом вниз на 72% времени, затем на место.
            _swing.anchoredPosition = Vector2.up * DropHeight * scale;

            animation
                .Append(_swing
                    .DOAnchorPosY(-DropOvershoot * scale, DropDuration * DropOvershootTime)
                    .SetEase(Ease.OutQuad))
                .Append(_swing
                    .DOAnchorPosY(0f, DropDuration * (1f - DropOvershootTime))
                    .SetEase(Ease.OutQuad));
        }

        // Общее скрытие попапов мгновенное: доска сама уезжает вверх (Р-24).
        protected override void ModifyHideAnimation(Sequence animation)
        {
            base.ModifyHideAnimation(animation);

            animation.Append(_swing
                .DOAnchorPosY(DropHeight * MainMenuLayout.MockupToReference, RaiseDuration)
                .SetEase(Ease.InQuad));
        }

        private float GetRowsHeight(int rows) => rows * _tileSize + (rows - 1) * TileGap;

        private void ApplyScroll()
        {
            if (_hasLayout == false || _isScrollable == false)
                return;

            float scale = MainMenuLayout.MockupToReference;
            int row = _scrollTargetIndex / Columns;
            float offset = Mathf.Clamp(row * (_tileSize + TileGap), 0f, _contentHeight - _viewHeight);

            _scrollRect.StopMovement();
            _scrollRect.content.anchoredPosition = new Vector2(0f, offset * scale);
        }

        private void OnCardHoverStarted(PlayModeCardView card) => GetOtherCard(card).SetDimmed(true);

        private void OnCardHoverEnded(PlayModeCardView card) => GetOtherCard(card).SetDimmed(false);

        private PlayModeCardView GetOtherCard(PlayModeCardView card) => card == _campaignCard ? _endlessCard : _campaignCard;

        private void OnCampaignCardClicked() => CampaignClicked?.Invoke();

        private void OnBackButtonClicked() => BackClicked?.Invoke();
    }
}
