using Assets._Project.Develop.Runtime.UI.Core.Views;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet
{
    // «+N» у числа золота (twrise макета, 1,6 с): проявляется за первые 15 %, поднимаясь из-под места на 9 u, держится до
    // 70 % и гаснет, уходя вверх на 51 u к концу.
    public class GoldGainView : MonoBehaviour, IView
    {
        private const float Duration = 1.6f;
        private const float AppearDuration = 0.24f;
        private const float FadeStartTime = 1.12f;
        private const float AppearOffset = 9f;
        private const float RiseHeight = 51f;

        [SerializeField] private TMP_Text _text;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _rect;

        private float _restY;

        private Sequence _animation;

        public void Show(int gold)
        {
            _text.text = $"+{gold}";

            KillAnimation();

            _group.alpha = 0f;
            _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, _restY - AppearOffset);

            _animation = DOTween.Sequence()
                .Insert(0f, _group.DOFade(1f, AppearDuration).SetEase(Ease.OutQuad))
                .Insert(0f, _rect.DOAnchorPosY(_restY, AppearDuration).SetEase(Ease.OutQuad))
                .Insert(AppearDuration, _rect.DOAnchorPosY(_restY + RiseHeight, Duration - AppearDuration).SetEase(Ease.OutQuad))
                .Insert(FadeStartTime, _group.DOFade(0f, Duration - FadeStartTime).SetEase(Ease.OutQuad))
                .SetUpdate(true);
        }

        private void Awake()
        {
            _restY = _rect.anchoredPosition.y;
            _group.alpha = 0f;
        }

        private void OnDestroy() => KillAnimation();

        private void KillAnimation() => _animation?.Kill();
    }
}
