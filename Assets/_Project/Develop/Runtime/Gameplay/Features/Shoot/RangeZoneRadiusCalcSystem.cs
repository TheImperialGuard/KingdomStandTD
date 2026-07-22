using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class RangeZoneRadiusCalcSystem : IInitializableSystem, IDisposableSystem
    {
        private ShootingRangeZone _rangeZone;
        private ReactiveVariable<float> _rangeDistance;

        private IDisposable _rangeDistanceDisposable;

        public void OnInit(Entity entity)
        {
            _rangeZone = entity.ShootingRangeZone;
            _rangeDistance = entity.InstantShootRange;

            _rangeZone.SetRange(_rangeDistance.Value);

            _rangeDistanceDisposable = _rangeDistance.Subscribe(OnRangeDistanceChanged);
        }

        private void OnRangeDistanceChanged(float oldDistance, float newDistance)
        {
            if (newDistance < 0)
                throw new ArgumentOutOfRangeException(nameof(newDistance));

            _rangeZone.SetRange(newDistance);
        }

        public void OnDispose()
        {
            _rangeDistanceDisposable.Dispose();
        }
    }
}
