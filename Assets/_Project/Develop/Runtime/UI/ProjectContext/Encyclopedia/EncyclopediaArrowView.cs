using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Стрелка листания внизу страницы: при наведении увеличивается, на крайнем развороте бледная и не нажимается
    // (spec-encyclopedia.md → «Листание»). Не Selectable (Р-25).
    public class EncyclopediaArrowView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
    {
        public event Action Clicked;

        [SerializeField] private RectTransform _icon;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField, Min(1f)] private float _hoveredScale = 1.25f;
        [SerializeField, Min(0f)] private float _scaleDuration = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _disabledAlpha = 0.25f;

        private Vector3 _idleScale;
        private bool _isInteractable = true;

        private Tween _scaleTween;

        private void Awake()
        {
            _idleScale = _icon.localScale;
        }

        private void OnDisable() => ResetScale();

        private void OnDestroy() => _scaleTween?.Kill();

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
            _canvasGroup.alpha = isInteractable ? 1f : _disabledAlpha;
            _canvasGroup.blocksRaycasts = isInteractable;

            if (isInteractable == false)
                ResetScale();
        }

        public void OnPointerEnter(PointerEventData eventData) => ScaleTo(_idleScale * _hoveredScale);

        public void OnPointerExit(PointerEventData eventData) => ScaleTo(_idleScale);

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isInteractable)
                Clicked?.Invoke();
        }

        private void ScaleTo(Vector3 scale)
        {
            _scaleTween?.Kill();
            _scaleTween = _icon.DOScale(scale, _scaleDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        private void ResetScale()
        {
            _scaleTween?.Kill();
            _icon.localScale = _idleScale;
        }
    }
}
