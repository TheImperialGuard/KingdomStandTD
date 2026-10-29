using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    // Иней на замороженной башне: на время заморозки мешам модели добавляется второй материал с коркой (Project/TowerFrost),
    // корка нарастает снизу вверх и после заморозки сходит сверху вниз, затем материал снимается.
    // Меши собираются под объектом вью: у вариантов башни они разные, а вью стоит на общем Origin
    public class FrozenTowerView : EntityView
    {
        private const float NoFrost = 0;
        private const float FullFrost = 1;

        private static readonly int _frostAmountId = Shader.PropertyToID("_FrostAmount");
        private static readonly int _frostBaseYId = Shader.PropertyToID("_FrostBaseY");
        private static readonly int _frostHeightId = Shader.PropertyToID("_FrostHeight");

        [SerializeField] private Material _frostMaterial;
        [SerializeField, Min(0)] private float _freezeDuration = 0.3f;
        [SerializeField, Min(0)] private float _thawDuration = 0.8f;

        private MaterialPropertyBlock _propertyBlock;
        private MeshRenderer[] _renderers;
        private Material[][] _originalMaterials;
        private Material[][] _frostedMaterials;

        private IReadOnlyVariable<bool> _isFrozen;
        private float _frostAmount;
        private bool _isFrostApplied;
        private Tween _tween;

        private IDisposable _isFrozenChangedDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            if (_isFrozen == null)
                return;

            _isFrozenChangedDisposable.Dispose();
            _isFrozen = null;

            KillTween();
            RemoveFrost();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            if (entity.TryGetIsFrozen(out ReactiveVariable<bool> isFrozen) == false)
                return;

            _isFrozen = isFrozen;

            CollectRenderers();

            _isFrozenChangedDisposable = _isFrozen.Subscribe(OnIsFrozenChanged);
        }

        private void OnIsFrozenChanged(bool oldValue, bool isFrozen)
        {
            KillTween();

            if (isFrozen)
            {
                ApplyFrost();
                _tween = DOTween.To(() => _frostAmount, SetFrostAmount, FullFrost, _freezeDuration);
            }
            else
            {
                _tween = DOTween
                    .To(() => _frostAmount, SetFrostAmount, NoFrost, _thawDuration)
                    .OnComplete(RemoveFrost);
            }
        }

        private void CollectRenderers()
        {
            if (_renderers != null)
                return;

            _propertyBlock = new MaterialPropertyBlock();
            _renderers =GetComponentsInChildren<MeshRenderer>(true);
            _originalMaterials = new Material[_renderers.Length][];
            _frostedMaterials = new Material[_renderers.Length][];

            for (int i = 0; i < _renderers.Length; i++)
            {
                Material[] original = _renderers[i].sharedMaterials;
                Material[] frosted = new Material[original.Length + 1];

                original.CopyTo(frosted, 0);
                frosted[original.Length] = _frostMaterial;

                _originalMaterials[i] = original;
                _frostedMaterials[i] = frosted;
            }
        }

        private void ApplyFrost()
        {
            if (_isFrostApplied)
                return;

            _isFrostApplied = true;

            // Граница корки считается в мире: от основания башни до верха самого высокого меша
            float baseY = transform.position.y;
            float topY = baseY;

            foreach (MeshRenderer renderer in _renderers)
                topY = Mathf.Max(topY, renderer.bounds.max.y);

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].sharedMaterials = _frostedMaterials[i];

            _propertyBlock.Clear();
            _propertyBlock.SetFloat(_frostBaseYId, baseY);
            _propertyBlock.SetFloat(_frostHeightId, topY - baseY);

            SetFrostAmount(NoFrost);
        }

        private void RemoveFrost()
        {
            if (_isFrostApplied == false)
                return;

            _isFrostApplied = false;
            _frostAmount = NoFrost;

            // Без блока свойств меши снова попадают в SRP Batcher
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].sharedMaterials = _originalMaterials[i];
                _renderers[i].SetPropertyBlock(null);
            }
        }

        private void SetFrostAmount(float frostAmount)
        {
            _frostAmount = frostAmount;
            _propertyBlock.SetFloat(_frostAmountId, frostAmount);

            foreach (MeshRenderer renderer in _renderers)
                renderer.SetPropertyBlock(_propertyBlock);
        }

        private void KillTween()
        {
            if (_tween != null && _tween.IsActive())
                _tween.Kill();

            _tween = null;
        }
    }
}
