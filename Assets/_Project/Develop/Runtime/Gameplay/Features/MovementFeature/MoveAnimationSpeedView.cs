using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    // Подстраивает темп клипа ходьбы под скорость движения через Speed Multiplier состояния ходьбы.
    [RequireComponent(typeof(Animator))]
    public class MoveAnimationSpeedView : EntityView
    {
        private const float DefaultMultiplier = 1;

        [SerializeField] private Animator _animator;
        [SerializeField] private MoveAnimationSpeedsConfig _config;
        [SerializeField] private AnimationClip _moveClip;

        [SerializeField] private string _speedMultiplierKeyName = "MoveSpeedMultiplier";

        private IReadOnlyVariable<float> _moveSpeed;

        private IDisposable _moveSpeedChangedDisposable;

#if UNITY_EDITOR
        private bool _isMissingClipReported;
#endif

        private int SpeedMultiplierKey => Animator.StringToHash(_speedMultiplierKeyName);

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _moveSpeedChangedDisposable?.Dispose();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _moveSpeed = entity.MoveSpeed;

            _moveSpeedChangedDisposable = _moveSpeed.Subscribe(OnMoveSpeedChanged);

#if UNITY_EDITOR
            _config.Changed += Apply;
#endif

            Apply();
        }

        private void OnValidate()
        {
            _animator = _animator != null ? _animator : GetComponent<Animator>();
        }

#if UNITY_EDITOR
        // Ассет таблицы переживает выход из Play Mode, а Cleanup при выходе не вызывается, поэтому отписка здесь
        private void OnDestroy() => _config.Changed -= Apply;
#endif

        private void OnMoveSpeedChanged(float oldSpeed, float speed) => Apply();

        private void Apply()
        {
            float multiplier = DefaultMultiplier;

            if (_config.TryGetBaseSpeed(_moveClip, out float baseSpeed))
                multiplier = _moveSpeed.Value / (baseSpeed * _animator.transform.lossyScale.y);
            else
                ReportMissingClip();

            _animator.SetFloat(SpeedMultiplierKey, multiplier);
        }

        private void ReportMissingClip()
        {
#if UNITY_EDITOR
            if (_isMissingClipReported)
                return;

            _isMissingClipReported = true;
            Debug.LogWarning($"Клипа ходьбы {_moveClip} нет в таблице {_config}: темп шага остаётся ×1", this);
#endif
        }
    }
}
