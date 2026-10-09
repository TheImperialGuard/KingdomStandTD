using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    // Показывает значки текущих статусов сущности; порядок значков задаёт список сопоставления.
    public class StatusIconsView : EntityView
    {
        [SerializeField] private List<StatusIcon> _icons;
        [SerializeField, Min(1)] private float _iconHeight = 110;

        private StatusesList _statuses;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            if (_statuses == null)
                return;

            _statuses.Added -= OnStatusesChanged;
            _statuses.Removed -= OnStatusesChanged;
            _statuses = null;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetStatuses(out StatusesList statuses) == false)
                return;

            _statuses = statuses;

            ResizeIcons();

            _statuses.Added += OnStatusesChanged;
            _statuses.Removed += OnStatusesChanged;

            Refresh();
        }

        private void ResizeIcons()
        {
            foreach (StatusIcon icon in _icons)
            {
                Rect spriteRect = icon.Image.sprite.rect;
                float width = _iconHeight * spriteRect.width / spriteRect.height;

                icon.Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                icon.Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _iconHeight);
            }
        }

        private void OnStatusesChanged(Status status) => Refresh();

        private void Refresh()
        {
            foreach (StatusIcon icon in _icons)
                icon.Image.gameObject.SetActive(_statuses.Has(icon.Type));
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
