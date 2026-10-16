using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Закладка раздела, торчащая из-под переплёта: при наведении выдвигается дальше, выбранная — ещё дальше и ярче
    // (spec-encyclopedia.md → «Закладки», Р-71). Не Selectable: навигации и залипшего выделения нет (Р-25).
    public class EncyclopediaBookmarkView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
    {
        public event Action<EncyclopediaSections> Clicked;

        [SerializeField] private EncyclopediaSections _section;
        [SerializeField] private RectTransform _tab;
        [SerializeField] private Image _image;

        [SerializeField] private float _hoveredShift = 9f;
        [SerializeField] private float _selectedShift = 18f;
        [SerializeField, Range(0f, 1f)] private float _idleBrightness = 0.82f;
        [SerializeField, Min(0f)] private float _transitionDuration = 0.15f;

        private float _idleX;
        private bool _isHovered;
        private bool _isSelected;

        private Sequence _transition;

        public EncyclopediaSections Section => _section;

        private void Awake()
        {
            _idleX = _tab.anchoredPosition.x;
            UpdateLook(isInstant: true);
        }

        private void OnDisable()
        {
            _isHovered = false;
            UpdateLook(isInstant: true);
        }

        private void OnDestroy() => _transition?.Kill();

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateLook(isInstant: false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateLook(isInstant: false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateLook(isInstant: false);
        }

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(_section);

        private void UpdateLook(bool isInstant)
        {
            float shift = 0f;

            if (_isSelected)
                shift = _selectedShift;
            else if (_isHovered)
                shift = _hoveredShift;

            float brightness = _isSelected || _isHovered ? 1f : _idleBrightness;
            Color color = new(brightness, brightness, brightness, _image.color.a);
            float x = _idleX + shift;

            _transition?.Kill();

            if (isInstant)
            {
                _tab.anchoredPosition = new Vector2(x, _tab.anchoredPosition.y);
                _image.color = color;
                return;
            }

            _transition = DOTween.Sequence()
                .Join(_tab.DOAnchorPosX(x, _transitionDuration).SetEase(Ease.OutQuad))
                .Join(_image.DOColor(color, _transitionDuration).SetEase(Ease.OutQuad))
                .SetUpdate(true);
        }
    }
}
