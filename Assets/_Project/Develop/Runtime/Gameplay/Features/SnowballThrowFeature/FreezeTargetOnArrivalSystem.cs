using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature
{
    // По прилёте снежка замораживает цель, если она ещё жива. Повторная заморозка перезапускает таймер статуса.
    public class FreezeTargetOnArrivalSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly StatusesFactory _statusesFactory;

        private Entity _snowball;

        private ReactiveVariable<Entity> _target;
        private ReactiveVariable<float> _freezeDuration;

        private IDisposable _arrivedDisposable;

        public FreezeTargetOnArrivalSystem(StatusesFactory statusesFactory)
        {
            _statusesFactory = statusesFactory;
        }

        public void OnInit(Entity entity)
        {
            _snowball = entity;

            _target = entity.FreezeTarget;
            _freezeDuration = entity.FreezeDuration;

            _arrivedDisposable = entity.IsArcFlightFinished.Subscribe(OnArcFlightFinishedChanged);
        }

        public void OnDispose()
        {
            _arrivedDisposable?.Dispose();
        }

        private void OnArcFlightFinishedChanged(bool oldValue, bool isFinished)
        {
            if (isFinished == false)
                return;

            Entity target = _target.Value;

            // Объект освобождённой башни мог уже достаться другой сущности — проверяем саму сущность
            if (target == null || target.IsInit == false)
                return;

            Status status = _statusesFactory.CreateFor(
                target,
                StatusesTypes.Freeze,
                _snowball,
                _freezeDuration.Value,
                _freezeDuration.Value);

            EntitiesHelper.TryInjectStatusTo(target, status);
        }
    }
}
