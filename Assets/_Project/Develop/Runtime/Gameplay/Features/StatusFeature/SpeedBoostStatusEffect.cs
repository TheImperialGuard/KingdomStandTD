using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class SpeedBoostStatusEffect : IStatusEffect
    {
        private readonly ReactiveVariable<float> _speedIndex;

        private IStatsEffect _statsEffect;

        public SpeedBoostStatusEffect(ReactiveVariable<float> speedIndex)
        {
            _speedIndex = speedIndex;
        }

        public void Apply(Entity entity)
        {
        }

        public void OnAdd(Entity entity)
        {
            if (entity.TryGetStatsEffects(out StatsEffectsList statsEffects) == false)
                return;

            float speedIndex = _speedIndex.Value;

            _statsEffect = new StatsEffect(StatTypes.MoveSpeed, speed => speed * speedIndex);

            statsEffects.AddElement(_statsEffect);
        }

        public void OnRemove(Entity entity)
        {
            if (_statsEffect == null)
                return;

            if (entity.TryGetStatsEffects(out StatsEffectsList statsEffects))
                statsEffects.RemoveElement(_statsEffect);

            _statsEffect = null;
        }
    }
}
