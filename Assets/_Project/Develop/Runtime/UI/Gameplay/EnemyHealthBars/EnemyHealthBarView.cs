using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.EnemyHealthBars
{
    // Полоска здоровья врага в экранном слое интерфейса: встаёт нижним краем (пивот корня) на точку над моделью,
    // над полоской — значки статусов; порядок значков задаёт список сопоставления
    public class EnemyHealthBarView : MonoBehaviour, IView, IWorldPositionView
    {
        [SerializeField] private Bar _bar;
        [SerializeField] private List<StatusIcon> _statusIcons;

        private Camera _camera;

        public void UpdateWorldPosition(Vector3 worldPosition)
        {
            Vector3 screenPosition = _camera.WorldToScreenPoint(worldPosition);

            // В z — расстояние до камеры. Вне плоскости канваса uGUI не сортирует полоски по глубине и рвёт батч
            screenPosition.z = 0f;
            transform.position = screenPosition;
        }

        public void SetValue(float value) => _bar.UpdateValue(value);

        public void SetStatusShown(StatusesTypes type, bool isShown)
        {
            foreach (StatusIcon icon in _statusIcons)
            {
                if (icon.Type == type)
                    icon.Image.gameObject.SetActive(isShown);
            }
        }

        public void HideStatuses()
        {
            foreach (StatusIcon icon in _statusIcons)
                icon.Image.gameObject.SetActive(false);
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        private void Awake()
        {
            _camera = Camera.main;
        }

        [Serializable]
        private class StatusIcon
        {
            [SerializeField] private StatusesTypes _type;
            [SerializeField] private Image _image;

            public StatusesTypes Type => _type;

            public Image Image => _image;
        }
    }
}
