using Assets._Project.Develop.Runtime.Utilities.Rendering;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    // Круг дальности башни на земле. Показывает текущую дальность, а при заряде улучшения — прирост до дальности
    // следующего уровня: внешняя пунктирная граница вырастает от текущей (Р-19).
    public class ShootingRangeZone : MonoBehaviour
    {
        // Запас квадрата за внешней границей, чтобы в него поместились кромка и сглаживание
        private const float QuadMargin = 0.1f;

        private static readonly int _radiusId = Shader.PropertyToID("_Radius");
        private static readonly int _outerRadiusId = Shader.PropertyToID("_OuterRadius");
        private static readonly int _opacityId = Shader.PropertyToID("_Opacity");

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField, Min(0)] private float _fadeInDuration = 0.2f;
        [SerializeField, Min(0)] private float _growthDuration = 0.2f;

        private RendererFader _fader;
        private MaterialPropertyBlock _propertyBlock;

        private float _range;
        private float _outerRange;
        private Tween _growthTween;

        public void SetRange(float range)
        {
            _range = range;

            ApplyOuterRange(range);
        }

        public void Show() => _fader.FadeIn(_fadeInDuration);

        public void Hide()
        {
            HideGrowth();
            _fader.Hide();
        }

        public void ShowGrowth(float nextRange)
        {
            KillGrowth();

            _growthTween = DOTween
                .To(() => _outerRange, ApplyOuterRange, nextRange, _growthDuration)
                .SetEase(Ease.OutQuad);
        }

        public void HideGrowth()
        {
            KillGrowth();
            ApplyOuterRange(_range);
        }

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _fader = new RendererFader(_renderer, _opacityId);
        }

        private void OnDestroy()
        {
            KillGrowth();
            _fader.Kill();
        }

        private void ApplyOuterRange(float outerRange)
        {
            _outerRange = outerRange;

            GroundQuadScaler.FitToRadius(_renderer.transform, Mathf.Max(_range, outerRange) + QuadMargin);

            // Блок читается с рендерера, чтобы не затереть прозрачность, которую выставляет RendererFader
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(_radiusId, _range);
            _propertyBlock.SetFloat(_outerRadiusId, outerRange);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void KillGrowth()
        {
            if (_growthTween != null && _growthTween.IsActive())
                _growthTween.Kill();

            _growthTween = null;
        }
    }
}
