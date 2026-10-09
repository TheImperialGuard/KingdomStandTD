using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Rendering;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WarCryFeature
{
    // Показывает зону боевого клича на время крика: плавно проявляет её в начале и гасит в конце крика или при смерти.
    public class WarCryZoneView : EntityView
    {
        private static readonly int _opacityId = Shader.PropertyToID("_Opacity");
        private static readonly int _startTimeId = Shader.PropertyToID("_StartTime");

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField, Min(0)] private float _fadeInDuration = 0.2f;
        [SerializeField, Min(0)] private float _fadeOutDuration = 0.3f;

        private RendererFader _fader;
        private MaterialPropertyBlock _propertyBlock;

        private IDisposable _abilityStartedDisposable;
        private IDisposable _abilityEndedDisposable;
        private IDisposable _isDeadChangedDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _abilityStartedDisposable?.Dispose();
            _abilityEndedDisposable?.Dispose();
            _isDeadChangedDisposable?.Dispose();

            _fader?.Kill();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetWarCryTargetKinds(out _) == false)
                return;

            FitToRadius(entity.AreaEffectRadius.Value);

            _propertyBlock = new MaterialPropertyBlock();
            _fader = new RendererFader(_renderer, _opacityId);

            _abilityStartedDisposable = entity.StartFirstAbilityEvent.Subscribe(OnAbilityStarted);
            _abilityEndedDisposable = entity.EndFirstAbilityEvent.Subscribe(OnAbilityEnded);
            _isDeadChangedDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        private void FitToRadius(float radius)
        {
            float diameter = radius * 2;
            Vector3 parentScale = transform.parent.lossyScale;

            // Квадрат лежит лицом вверх, поэтому его локальная ось Y идёт вдоль оси Z родителя
            transform.localScale = new Vector3(diameter / parentScale.x, diameter / parentScale.z, transform.localScale.z);
        }

        private void OnAbilityStarted()
        {
            // Отсчёт волн идёт от начала крика, чтобы первая волна выходила из центра
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(_startTimeId, Time.timeSinceLevelLoad);
            _renderer.SetPropertyBlock(_propertyBlock);

            _fader.FadeIn(_fadeInDuration);
        }

        private void OnAbilityEnded() => _fader.FadeOut(_fadeOutDuration);

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead)
                _fader.FadeOut(_fadeOutDuration);
        }
    }
}
