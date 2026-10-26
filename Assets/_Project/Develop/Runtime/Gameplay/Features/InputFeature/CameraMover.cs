using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.CameraManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class CameraMover
    {
        private const float NoZoom = 1f;

        private readonly BoundedOrthoCamera _boundedCamera;
        private readonly IInputService _inputService;
        private readonly CameraConfig _cameraConfig;

        public CameraMover(BoundedOrthoCamera boundedCamera, IInputService inputService, CameraConfig cameraConfig)
        {
            _boundedCamera = boundedCamera;
            _inputService = inputService;
            _cameraConfig = cameraConfig;
        }

        public void Update()
        {
            _boundedCamera.RefitOnAspectChange();

            Vector3 delta = _inputService.CameraDelta;

            if (delta != Vector3.zero)
                _boundedCamera.Move(delta);

            float zoomFactor = _inputService.CameraZoomFactor;

            // Предел приближения читается из конфига на каждом шаге зума: его можно подбирать прямо в Play Mode
            if (Mathf.Approximately(zoomFactor, NoZoom) == false)
                _boundedCamera.Zoom(zoomFactor, _inputService.CameraZoomPivot, _cameraConfig.MaxZoom);
        }
    }
}
