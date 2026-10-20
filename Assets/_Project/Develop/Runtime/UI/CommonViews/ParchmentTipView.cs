using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Подсказка на пергаменте: ширина по содержимому в пределах [min, max] (CSS shrink-to-fit), высота — по тексту.
    // Корень ставит владелец, тело висит от его верха и появляется twfade макета: проявление и сдвиг сверху вниз.
    // Строки содержимого — в префабе владельца, здесь только раскладка и появление.
    public class ParchmentTipView : MonoBehaviour
    {
        private const float AppearDuration = 0.15f;
        private const float AppearOffset = 9f;

        [SerializeField] private RectTransform _body;
        [SerializeField] private CanvasGroup _group;

        [SerializeField, Min(0f)] private float _minWidth = 450f;
        [SerializeField, Min(0f)] private float _maxWidth = 780f;

        private Sequence _appearAnimation;

        public void Show()
        {
            gameObject.SetActive(true);
            Refresh();

            KillAppearAnimation();

            _group.alpha = 0f;
            _body.anchoredPosition = Vector2.down * AppearOffset;

            _appearAnimation = DOTween.Sequence()
                .Insert(0f, _group.DOFade(1f, AppearDuration))
                .Insert(0f, _body.DOAnchorPos(Vector2.zero, AppearDuration))
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        public void Hide()
        {
            KillAppearAnimation();
            gameObject.SetActive(false);
        }

        // Зовётся после смены текста: ширина и высота пересчитываются сразу, а не в следующем кадре.
        public void Refresh()
        {
            if (gameObject.activeInHierarchy == false)
                return;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_body);

            float width = Mathf.Clamp(LayoutUtility.GetPreferredWidth(_body), _minWidth, _maxWidth);

            _body.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_body);
        }

        private void OnDestroy() => KillAppearAnimation();

        private void KillAppearAnimation() => _appearAnimation?.Kill();
    }
}
