using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    // Появление башни: на I уровне её построили, на II–IV — улучшили. Улучшение заменяет башню новой,
    // поэтому эффект показывает новая башня, когда начинает работу. Башня-превью при выборе постройки эффекта не показывает
    public class TowerAppearanceView : ParticleEffectsEntityView
    {
        private const int BuiltTowerLevel = 1;

        [SerializeField] private TowerAppearanceEffectsConfig _config;
        [SerializeField] private Transform _model;
        [SerializeField] private Transform _topPoint;

        private Tween _popTween;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            // Башня, убранная посреди пружины, возвращает модели обычный масштаб
            if (_popTween != null && _popTween.IsActive())
                _popTween.Kill(true);

            _popTween = null;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            if (entity.TowerLevel.Value == BuiltTowerLevel)
            {
                EffectsPool.Spawn(_config.BuildEffect, transform.position);
            }
            else
            {
                EffectsPool.Spawn(_config.UpgradeEffect, transform.position);
                EffectsPool.Spawn(_config.UpgradeTopEffect, _topPoint.position);
            }

            if (_config.IsPopEnabled == true)
                PlayPop();
        }

        private void PlayPop()
        {
            Vector3 scale = _model.localScale;

            _model.localScale = Vector3.Scale(scale, _config.PopStartScale);
            _popTween = _model.DOScale(scale, _config.PopDuration).SetEase(Ease.OutBack);
        }
    }
}
