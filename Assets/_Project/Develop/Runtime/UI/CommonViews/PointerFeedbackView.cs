using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Отклик на указатель: при наведении увеличивает одну часть, при нажатии опускает другую. Отключённый Selectable
    // отклика не даёт, как :not(:disabled) в макете.
    public class PointerFeedbackView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private Selectable _selectable;
        [SerializeField] private RectTransform _scaleTarget;
        [SerializeField] private RectTransform _pressTarget;

        [SerializeField, Min(1f)] private float _hoveredScale = 1.08f;
        [SerializeField] private float _pressedOffset = 6f;
        [SerializeField, Min(0f)] private float _duration = 0.12f;

        private Vector2 _pressIdlePosition;

        private Tween _scaleTween;
        private Tween _pressTween;

        private bool _isHovered;
        private bool _isPressed;

        private bool IsInteractable => _selectable == null || _selectable.IsInteractable();

        private void Awake()
        {
            _pressIdlePosition = _pressTarget.anchoredPosition;
        }

        private void OnDisable()
        {
            _isHovered = false;
            _isPressed = false;

            KillTweens();
            _scaleTarget.localScale = Vector3.one;
            _pressTarget.anchoredPosition = _pressIdlePosition;
        }

        private void OnDestroy() => KillTweens();

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateScale();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateScale();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPressed = true;
            UpdatePress();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPressed = false;
            UpdatePress();
        }

        // Зовётся, когда у Selectable поменялась доступность: отклик должен сразу ей соответствовать.
        public void Refresh()
        {
            UpdateScale();
            UpdatePress();
        }

        private void UpdateScale()
        {
            float scale = _isHovered && IsInteractable ? _hoveredScale : 1f;

            _scaleTween?.Kill();

            _scaleTween = _scaleTarget
                .DOScale(scale, _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        private void UpdatePress()
        {
            Vector2 position = _isPressed && IsInteractable
                ? _pressIdlePosition + Vector2.down * _pressedOffset
                : _pressIdlePosition;

            _pressTween?.Kill();

            _pressTween = _pressTarget
                .DOAnchorPos(position, _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        private void KillTweens()
        {
            _scaleTween?.Kill();
            _pressTween?.Kill();
        }
    }
}
