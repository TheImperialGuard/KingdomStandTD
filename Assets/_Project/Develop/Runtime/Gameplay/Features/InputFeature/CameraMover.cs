using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class CameraMover
    {
        private readonly BoundedOrthoCamera _boundedCamera;
        private readonly IInputService _inputService;

        public CameraMover(BoundedOrthoCamera boundedCamera, IInputService inputService)
        {
            _boundedCamera = boundedCamera;
            _inputService = inputService;
        }

        public void Update()
        {
            Vector3 delta = _inputService.CameraDelta;

            if (delta != Vector3.zero)
                _boundedCamera.MoveCamera(delta);
        }
    }
}
