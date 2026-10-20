using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups
{
    // Место боковой подсказки окна башни (генератор макета, D_Towers): сбоку от кольца на стороне, противоположной половине
    // экрана с башней, по вертикали — по центру кольца; прижимается к краям safe area. Координаты — в единицах канваса,
    // y вверх.
    public static class TowerTipLayout
    {
        private const float RingGap = 240f;
        private const float SideMargin = 12f;
        private const float TopMargin = 18f;
        private const float BottomMargin = 12f;

        // Возвращает левый верхний угол подсказки.
        public static Vector2 GetTopLeft(Vector2 ringCenter, Vector2 tipSize, Rect screen, Rect safeArea)
        {
            bool isOnRight = ringCenter.x < screen.center.x;

            float left = isOnRight ? ringCenter.x + RingGap : ringCenter.x - RingGap - tipSize.x;
            left = Mathf.Clamp(left, safeArea.xMin + SideMargin, safeArea.xMax - SideMargin - tipSize.x);

            float top = Mathf.Max(safeArea.yMin + BottomMargin + tipSize.y, ringCenter.y + tipSize.y / 2f);
            top = Mathf.Min(safeArea.yMax - TopMargin, top);

            return new Vector2(left, top);
        }
    }
}
