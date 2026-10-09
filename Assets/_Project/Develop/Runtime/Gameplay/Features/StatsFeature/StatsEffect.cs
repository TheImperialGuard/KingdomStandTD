using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class StatsEffect : IStatsEffect
    {
        private readonly StatTypes _statType;
        private readonly Func<float, float> _applyEffect;

        public StatsEffect(StatTypes statType, Func<float, float> applyEffect)
        {
            _statType = statType;
            _applyEffect = applyEffect;
        }

        public void ApplyTo(Dictionary<StatTypes, float> stats)
        {
            if (stats.TryGetValue(_statType, out float value) == false)
                return;

            stats[_statType] = _applyEffect(value);
        }
    }
}
