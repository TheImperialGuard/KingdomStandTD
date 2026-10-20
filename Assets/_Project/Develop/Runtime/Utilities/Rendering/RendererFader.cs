using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Rendering
{
    // Плавно показывает и скрывает рендерер, меняя свойство прозрачности его шейдера через MaterialPropertyBlock.
    public class RendererFader
    {
        private const float HiddenOpacity = 0;
        private const float VisibleOpacity = 1;

        private readonly Renderer _renderer;
        private readonly int _opacityId;
        private readonly MaterialPropertyBlock _propertyBlock = new();

        private float _opacity;
        private Tween _tween;

        public RendererFader(Renderer renderer, int opacityId)
        {
            _renderer = renderer;
            _opacityId = opacityId;

            SetOpacity(HiddenOpacity);
        }

        public void FadeIn(float duration)
        {
            Kill();

            _renderer.enabled = true;
            _tween = DOTween.To(() => _opacity, SetOpacity, VisibleOpacity, duration);
        }

        public void FadeOut(float duration)
        {
            Kill();

            _tween = DOTween
                .To(() => _opacity, SetOpacity, HiddenOpacity, duration)
                .OnComplete(Disable);
        }

        public void Hide()
        {
            Kill();
            SetOpacity(HiddenOpacity);
            Disable();
        }

        public void Kill()
        {
            if (_tween != null && _tween.IsActive())
                _tween.Kill();

            _tween = null;
        }

        private void SetOpacity(float opacity)
        {
            _opacity = opacity;

            // Блок читается с рендерера, чтобы не затереть другие свойства, выставленные через него
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(_opacityId, opacity);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void Disable() => _renderer.enabled = false;
    }
}
