using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class StatEffectsApplierSystem : IInitializableSystem, IDisposableSystem
    {
        private StatsEffectsList _statsEffects;
        private Dictionary<StatTypes, float> _baseStats;
        private Dictionary<StatTypes, float> _modifiedStats;

        public void OnInit(Entity entity)
        {
            _statsEffects = entity.StatsEffects;
            _baseStats = entity.BaseStats;
            _modifiedStats = entity.ModifiedStats;

            _statsEffects.Added += OnStatsEffectAdded;
            _statsEffects.Removed += OnStatsEffectRemoved;

            RecalculateStats();
        }

        public void OnDispose()
        {
            _statsEffects.Added -= OnStatsEffectAdded;
            _statsEffects.Removed -= OnStatsEffectRemoved;
        }

        private void OnStatsEffectAdded(IStatsEffect statsEffect) => RecalculateStats();

        private void OnStatsEffectRemoved(IStatsEffect statsEffect) => RecalculateStats();

        private void RecalculateStats()
        {
            foreach (KeyValuePair<StatTypes, float> baseStat in _baseStats)
                _modifiedStats[baseStat.Key] = baseStat.Value;

            foreach (IStatsEffect statsEffect in _statsEffects.Elements)
                statsEffect.ApplyTo(_modifiedStats);
        }
    }
}
