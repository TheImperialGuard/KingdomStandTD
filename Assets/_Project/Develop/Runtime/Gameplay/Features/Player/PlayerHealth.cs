using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Player
{
    public class PlayerHealth
    {
        private ReactiveVariable<int> _max = new();
        private ReactiveVariable<int> _current = new();

        public PlayerHealth(int maxValue, int currentValue)
        {
            _max.Value = maxValue;
            _current.Value = currentValue;
        }

        public IReadOnlyVariable<int> Max => _max;
        public IReadOnlyVariable<int> Current => _current;

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

            if (damage >= _current.Value)
                _current.Value = 0;
            else
                _current.Value -= damage;
        }
    }
}
