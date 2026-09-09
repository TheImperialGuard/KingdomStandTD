using Assets._Project.Develop.Runtime.Configs.Gameplay;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class CameraMover
    {
        private readonly BoundedOrthoCamera _boundedCamera;
        private readonly CameraConfig _cameraConfig;
        private readonly IInputService _inputService;

        public CameraMover(BoundedOrthoCamera boundedCamera, CameraConfig cameraConfig, IInputService inputService)
        {
            _boundedCamera = boundedCamera;
            _cameraConfig = cameraConfig;
            _inputService = inputService;
        }

        public void Update(float deltaTime)
        {
            if (_inputService.CameraDelta != Vector3.zero)
            {
                _boundedCamera.MoveCamera(_inputService.CameraDelta * _cameraConfig.MoveSpeed * deltaTime);
            }
        }
    }
}
