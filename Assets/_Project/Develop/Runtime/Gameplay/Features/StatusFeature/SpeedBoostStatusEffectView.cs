using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Rendering;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    // Показывает кружок под сущностью, пока на ней ускорение; гасит его при снятии статуса или смерти.
    public class SpeedBoostStatusEffectView : EntityView
    {
        private static readonly int _opacityId = Shader.PropertyToID("_Opacity");

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField, Min(0)] private float _radius = 0.24f;
        [SerializeField, Min(0)] private float _fadeInDuration = 0.2f;
        [SerializeField, Min(0)] private float _fadeOutDuration = 0.3f;

        private StatusesList _statuses;
        private IReadOnlyVariable<bool> _isDead;
        private RendererFader _fader;
        private bool _isShown;

        private IDisposable _isDeadChangedDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            if (_statuses == null)
                return;

            _statuses.Added -= OnStatusesChanged;
            _statuses.Removed -= OnStatusesChanged;
            _statuses = null;

            _isDeadChangedDisposable.Dispose();

            // Кружок гаснет сразу: объект из пула достанется новой сущности без ускорения
            _fader.Hide();
            _isShown = false;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetStatuses(out StatusesList statuses) == false)
                return;

            _statuses = statuses;
            _isDead = entity.IsDead;

            GroundQuadScaler.FitToRadius(transform, _radius);

            _fader = new RendererFader(_renderer, _opacityId);

            _statuses.Added += OnStatusesChanged;
            _statuses.Removed += OnStatusesChanged;
            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);

            Refresh();
        }

        private void OnStatusesChanged(Status status) => Refresh();

        private void OnIsDeadChanged(bool oldValue, bool isDead) => Refresh();

        private void Refresh()
        {
            bool mustShow = _isDead.Value == false && _statuses.Has(StatusesTypes.SpeedBoost);

            // Зона WarCry продлевает статус повторными Added: переключаемся только при смене состояния
            if (mustShow == _isShown)
                return;

            _isShown = mustShow;

            if (mustShow)
                _fader.FadeIn(_fadeInDuration);
            else
                _fader.FadeOut(_fadeOutDuration);
        }
    }
}
