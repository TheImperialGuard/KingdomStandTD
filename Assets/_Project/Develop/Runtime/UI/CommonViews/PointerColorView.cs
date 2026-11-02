using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Цвет наведения для нескольких Graphic сразу: переход ColorTint у Selectable красит только один, а у кнопки
    // чернилами их два — текст и подчёркивание одного цвета. Цвет меняется сразу, без перехода. Отключённый Selectable
    // не подсвечивается.
    public class PointerColorView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private Selectable _selectable;
        [SerializeField] private List<Graphic> _graphics;
        [SerializeField] private Color _idleColor = Color.white;
        [SerializeField] private Color _hoveredColor = Color.white;

        private bool _isHovered;

        private bool IsInteractable => _selectable == null || _selectable.IsInteractable();

        private void OnDisable()
        {
            _isHovered = false;
            UpdateColor();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateColor();
        }

        // Зовётся, когда у Selectable поменялась доступность: цвет должен сразу ей соответствовать.
        public void Refresh() => UpdateColor();

        private void UpdateColor()
        {
            Color color = _isHovered && IsInteractable ? _hoveredColor : _idleColor;

            foreach (Graphic graphic in _graphics)
                graphic.color = color;
        }
    }
}
