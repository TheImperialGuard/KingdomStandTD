using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups
{
    // Окно башни: кольцо с медальонами на башне в мире и боковая подсказка. Кольцо следует за башней и у края экрана не
    // сдвигается, подсказка прижимается к краям safe area. Появление — twpop макета: кольцо сразу, кнопки по очереди.
    public abstract class TowerPopupViewBase : PopupViewBase, IWorldPositionView
    {
        private const float AppearDuration = 0.18f;
        private const float AppearStartScale = 0.6f;
        private const float AppearStepDelay = 0.04f;

        [SerializeField] private CanvasGroup _ringAppear;
        [SerializeField] private List<CanvasGroup> _buttonsAppear;
        [SerializeField] private TowerTipView _tip;

        private Camera _camera;

        public void UpdateWorldPosition(Vector3 worldPosition)
        {
            Vector3 screenPosition = _camera.WorldToScreenPoint(worldPosition);

            // В z — расстояние до камеры. Вне плоскости канваса uGUI не сортирует окно по глубине и рвёт батч на каждой
            // смене материала.
            screenPosition.z = 0f;
            transform.position = screenPosition;

            if (_tip.IsShown)
                PlaceTip();
        }

        public void ShowTip(TowerTipContent content)
        {
            _tip.Show(content);
            PlaceTip();
        }

        public void HideTip() => _tip.Hide();

        protected override void ModifyShowAnimation(Sequence animation)
        {
            AppendAppear(animation, _ringAppear, 0f);

            for (int i = 0; i < _buttonsAppear.Count; i++)
                AppendAppear(animation, _buttonsAppear[i], i * AppearStepDelay);
        }

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void AppendAppear(Sequence animation, CanvasGroup group, float delay)
        {
            group.alpha = 0f;
            group.transform.localScale = Vector3.one * AppearStartScale;

            animation
                .Insert(delay, group.DOFade(1f, AppearDuration).SetEase(Ease.OutQuad))
                .Insert(delay, group.transform.DOScale(1f, AppearDuration).SetEase(Ease.OutQuad));
        }

        // Расчёт — в координатах слоя попапов: он растянут на весь канвас.
        private void PlaceTip()
        {
            RectTransform layer = (RectTransform)transform.parent;
            Vector2 ringCenter = layer.InverseTransformPoint(transform.position);
            Vector2 tipSize = _tip.Size;

            Vector2 topLeft = TowerTipLayout.GetTopLeft(ringCenter, tipSize, layer.rect, GetSafeArea(layer));
            Vector2 topCenter = topLeft + Vector2.right * (tipSize.x / 2f);

            _tip.transform.position = layer.TransformPoint(topCenter);
        }

        private Rect GetSafeArea(RectTransform layer)
        {
            Rect safeArea = Screen.safeArea;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(layer, safeArea.min, null, out Vector2 min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(layer, safeArea.max, null, out Vector2 max);

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
