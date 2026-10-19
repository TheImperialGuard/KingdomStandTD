using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    // Поднимает элемент, стоящий от нижнего края экрана, на нижний отступ safe area — чтобы он не уходил в жестовую зону
    // телефона. Отступ переводится в единицы канваса и пересчитывается при смене safe area или масштаба канваса.
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaBottomInset : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Canvas _rootCanvas;
        private float _baseY;
        private float _appliedInset;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _baseY = _rectTransform.anchoredPosition.y;
        }

        private void Update()
        {
            if (_rootCanvas == null)
                _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

            float inset = Screen.safeArea.yMin / _rootCanvas.scaleFactor;

            if (Mathf.Approximately(inset, _appliedInset))
                return;

            _appliedInset = inset;
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, _baseY + inset);
        }
    }
}
