using Assets._Project.Develop.Runtime.UI.CommonViews;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    public static class UIHelper
    {
        public static RelativeUIPositions GetRelativePositionFor(
        RectTransform uiElement)
        {
            Vector2 screenPosition =
                RectTransformUtility.WorldToScreenPoint(
                    null,
                    uiElement.TransformPoint(uiElement.rect.center));

            float screenCenterX = Screen.width * 0.5f;

            return screenPosition.x <= screenCenterX
                ? RelativeUIPositions.Left
                : RelativeUIPositions.Right;
        }
    }
}
