using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    // Строка левого меню: иконка и подпись на тёмной полосе. Наведение и выбор подсвечивают строку, у выбранной на конце
    // покачивается маркер. Не Selectable: навигации и залипшего выделения нет (Р-25).
    public class MainMenuItemView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
    {
        private static readonly Vector2 _centerAnchor = new(0.5f, 0.5f);

        public event Action Clicked;

        [SerializeField] private Image _band;
        [SerializeField] private GameObject _edges;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _iconSlot;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _marker;

        [SerializeField, Range(0f, 1f)] private float _bandIdleAlpha = 0.42f;
        [SerializeField, Range(0f, 1f)] private float _bandHighlightedAlpha = 0.72f;
        [SerializeField, Min(1f)] private float _labelHighlightedScale = 1.08f;
        [SerializeField, Min(1f)] private float _labelHoveredBrightness = 1.08f;
        [SerializeField, Min(0f)] private float _labelScaleDuration = 0.12f;
        [SerializeField] private float _pressedOffset = 6f;
        [SerializeField] private float _markerNudgeOffset = 7.5f;
        [SerializeField, Min(0.01f)] private float _markerNudgeStepDuration = 0.4f;

        private Color _labelIdleColor;
        private Vector2 _contentIdlePosition;
        private Vector2 _iconSlotAnchorMin;
        private Vector2 _iconSlotAnchorMax;
        private Vector2 _iconSlotPivot;
        private Vector2 _iconSlotPosition;
        private float _markerIdleX;

        private Tween _labelScaleTween;
        private Sequence _markerNudge;

        private bool _isHovered;
        private bool _isSelected;

        private void Awake()
        {
            _labelIdleColor = _label.color;
            _contentIdlePosition = _content.anchoredPosition;
            _iconSlotAnchorMin = _iconSlot.anchorMin;
            _iconSlotAnchorMax = _iconSlot.anchorMax;
            _iconSlotPivot = _iconSlot.pivot;
            _iconSlotPosition = _iconSlot.anchoredPosition;
            _markerIdleX = _marker.anchoredPosition.x;

            _marker.gameObject.SetActive(false);
            UpdateHighlight();
        }

        private void OnDisable()
        {
            _isHovered = false;
            _content.anchoredPosition = _contentIdlePosition;
            UpdateHighlight();
        }

        private void OnDestroy()
        {
            _labelScaleTween?.Kill();
            _markerNudge?.Kill();
        }

        public void SetSelected(bool isSelected)
        {
            if (_isSelected == isSelected)
                return;

            _isSelected = isSelected;
            _marker.gameObject.SetActive(isSelected);

            if (isSelected)
                StartMarkerNudge();
            else
                _markerNudge?.Kill();

            UpdateHighlight();
        }

        // Свёрнутое меню — только иконка по центру строки; ширину строки задаёт родитель.
        public void SetCollapsed(bool isCollapsed)
        {
            _label.gameObject.SetActive(isCollapsed == false);

            if (isCollapsed)
            {
                _iconSlot.anchorMin = _centerAnchor;
                _iconSlot.anchorMax = _centerAnchor;
                _iconSlot.pivot = _centerAnchor;
                _iconSlot.anchoredPosition = Vector2.zero;
                return;
            }

            _iconSlot.anchorMin = _iconSlotAnchorMin;
            _iconSlot.anchorMax = _iconSlotAnchorMax;
            _iconSlot.pivot = _iconSlotPivot;
            _iconSlot.anchoredPosition = _iconSlotPosition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateHighlight();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateHighlight();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _content.anchoredPosition = _contentIdlePosition + Vector2.down * _pressedOffset;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _content.anchoredPosition = _contentIdlePosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                Clicked?.Invoke();
        }

        private void UpdateHighlight()
        {
            bool isHighlighted = _isHovered || _isSelected;

            Color bandColor = _band.color;
            bandColor.a = isHighlighted ? _bandHighlightedAlpha : _bandIdleAlpha;
            _band.color = bandColor;

            _edges.SetActive(isHighlighted);

            // Цвет в uGUI не осветляет выше 1: светлеют только неполные каналы (Р-50).
            Color labelColor = _isHovered ? _labelIdleColor * _labelHoveredBrightness : _labelIdleColor;
            labelColor.a = _labelIdleColor.a;
            _label.color = labelColor;

            _labelScaleTween?.Kill();

            _labelScaleTween = _label.rectTransform
                .DOScale(isHighlighted ? _labelHighlightedScale : 1f, _labelScaleDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        // Маркер переступает ступенькой туда и обратно, как steps(1, end) в макете.
        private void StartMarkerNudge()
        {
            _markerNudge?.Kill();
            SetMarkerOffset(0f);

            _markerNudge = DOTween.Sequence()
                .AppendInterval(_markerNudgeStepDuration)
                .AppendCallback(() => SetMarkerOffset(_markerNudgeOffset))
                .AppendInterval(_markerNudgeStepDuration)
                .AppendCallback(() => SetMarkerOffset(0f))
                .SetLoops(-1)
                .SetUpdate(true);
        }

        private void SetMarkerOffset(float offset)
        {
            Vector2 position = _marker.anchoredPosition;
            position.x = _markerIdleX + offset;
            _marker.anchoredPosition = position;
        }
    }
}
