using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class AttackCooldownByAttackPerSecondStatSyncSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _attackPerSecond;
        private ReactiveVariable<float> _attackCooldownInitialTime;

        private IDisposable _attackPerSecondChangedDisposable;

        public void OnInit(Entity entity)
        {
            _attackPerSecond = entity.AttacksPerSecond;
            _attackCooldownInitialTime = entity.AttackCooldownInitialTime;

            _attackPerSecondChangedDisposable = _attackPerSecond.Subscribe(OnAttackPerSecondChanged);
            OnAttackPerSecondChanged(0, _attackPerSecond.Value);
        }

        public void OnDispose()
        {
            _attackPerSecondChangedDisposable.Dispose();
        }

        private void OnAttackPerSecondChanged(float oldAttackPerSecond, float newAttackPerSecond)
        {
            if (newAttackPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(newAttackPerSecond));

            float cooldown = 1f / newAttackPerSecond;

            _attackCooldownInitialTime.Value = cooldown;
        }
    }
}
