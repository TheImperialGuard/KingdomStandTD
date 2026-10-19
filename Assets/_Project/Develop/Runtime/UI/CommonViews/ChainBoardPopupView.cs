using Assets._Project.Develop.Runtime.UI.Core.Popups;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Доска на цепях (Р-59): панель, которая падает сверху и уезжает вверх при закрытии. Ширину, верх и правый отступ
    // задаёт расчёт раскладки сцены, содержимое доски — дело наследника. Числа — в px макета.
    public abstract class ChainBoardPopupView : PopupViewBase
    {
        private const float MockupToReference = 1.5f;
        private const float ChainOverlap = 20f;
        private const float DropHeight = 560f;
        private const float DropOvershoot = 12f;
        private const float DropDuration = 0.5f;
        private const float DropOvershootTime = 0.72f;
        private const float RaiseDuration = 0.2f;

        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _swing;
        [SerializeField] private RectTransform _board;
        [SerializeField] private RectTransform _leftChain;
        [SerializeField] private RectTransform _rightChain;

        protected bool HasLayout { get; private set; }

        protected float PanelWidth { get; private set; }

        protected RectTransform Board => _board;

        public void SetLayout(float panelWidth, float panelTop, float panelRightMargin)
        {
            HasLayout = true;
            PanelWidth = panelWidth;

            _panel.anchoredPosition = new Vector2(-panelRightMargin, 0f);
            _panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
            _board.anchoredPosition = new Vector2(0f, -panelTop);

            float chainHeight = panelTop + ChainOverlap * MockupToReference;
            _leftChain.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chainHeight);
            _rightChain.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chainHeight);

            OnLayoutApplied();
        }

        // Зовётся после каждой смены раскладки: размеры доски уже выставлены, содержимое наследник подгоняет сам.
        protected virtual void OnLayoutApplied() { }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            float scale = MockupToReference;

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
                .DOAnchorPosY(DropHeight * MockupToReference, RaiseDuration)
                .SetEase(Ease.InQuad));
        }
    }
}
