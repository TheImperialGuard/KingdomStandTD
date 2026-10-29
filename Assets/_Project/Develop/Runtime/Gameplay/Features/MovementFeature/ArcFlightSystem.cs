using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    // Полёт дугой по времени, без физики: по прямой от начальной точки к конечной плюс подъём по параболе.
    public class ArcFlightSystem : IInitializableSystem, IUpdatableSystem
    {
        // Подъём 4·h·t·(1 − t): при t = 0,5 он равен высоте дуги h.
        private const float ParabolaPeakFactor = 4;

        private Transform _transform;

        private ReactiveVariable<Vector3> _startPoint;
        private ReactiveVariable<Vector3> _endPoint;
        private ReactiveVariable<float> _height;
        private ReactiveVariable<float> _duration;
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<bool> _isFinished;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;

            _startPoint = entity.ArcFlightStartPoint;
            _endPoint = entity.ArcFlightEndPoint;
            _height = entity.ArcFlightHeight;
            _duration = entity.ArcFlightDuration;
            _currentTime = entity.ArcFlightCurrentTime;
            _isFinished = entity.IsArcFlightFinished;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isFinished.Value)
                return;

            _currentTime.Value += deltaTime;

            float t = Mathf.Clamp01(_currentTime.Value / _duration.Value);
            float lift = ParabolaPeakFactor * _height.Value * t * (1 - t);

            _transform.position = Vector3.Lerp(_startPoint.Value, _endPoint.Value, t) + Vector3.up * lift;

            if (t >= 1)
                _isFinished.Value = true;
        }
    }
}
