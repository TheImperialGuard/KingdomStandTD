using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Raycast
{
    public class RayShooterService
    {
        private readonly IInputService _inputService;

        private ReactiveVariable<RaycastHit> _lastHitInfo = new();

        public RayShooterService(IInputService inputService)
        {
            _inputService = inputService;
        }

        public IReadOnlyVariable<RaycastHit> LastHitInfo => _lastHitInfo;

        public void Update(float deltaTime)
        {
            if (_inputService.RayShotRequested == true)
                Shoot(_inputService.CameraRay.origin, _inputService.CameraRay.direction);
        }

        public void Cleanup()
        {
            _lastHitInfo.Value = new();
        }

        private void Shoot(Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);

            Physics.Raycast(ray, out RaycastHit hitInfo);

            _lastHitInfo.Value = hitInfo;

            Debug.Log($"Был выпущен луч. Поражена цель: {_lastHitInfo.Value.collider.gameObject.name}");
        }
    }
}
