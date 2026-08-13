using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class Status
    {
        private readonly StatusesTypes _type;

        private readonly IStatusEffect _statusEffect;

        private readonly Entity _source;

        private readonly ReactiveVariable<float> _initialTime;
        private readonly ReactiveVariable<float> _interval;

        public Status(IStatusEffect statusEffect, Entity source, float initialTime, float interval, StatusesTypes type)
        {
            _statusEffect = statusEffect;
            _source = source;
            _initialTime = new ReactiveVariable<float>(initialTime);
            _interval = new ReactiveVariable<float>(interval);
            _type = type;
        }

        public IReadOnlyVariable<float> InitialTime => _initialTime;

        public IReadOnlyVariable<float> Interval => _interval;

        public StatusesTypes Type => _type;

        public void ApplyEffect() => _statusEffect.Apply(_source);
    }
}
