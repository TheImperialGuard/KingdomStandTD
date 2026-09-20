using Assets._Project.Develop.Runtime.Configs.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const KeyCode RAY_SHOOTER_KEY_CODE = KeyCode.Mouse0;

        private readonly CameraConfig _cameraConfig;

        public DesktopInput(CameraConfig cameraConfig)
        {
            _cameraConfig = cameraConfig;
        }

        public bool IsEnabled { get; set; } = true;

        public bool RayShotRequested
            => IsEnabled && Input.GetKeyDown(RAY_SHOOTER_KEY_CODE) && IsPointerOverUI() == false;

        public Ray CameraRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        public Vector3 CameraDelta
        {
            get
            {
                if (IsEnabled == false)
                    return Vector3.zero;

                Vector3 direction = Vector3.zero;

                if (Input.GetKey(KeyCode.W))
                    direction += Vector3.forward;

                if (Input.GetKey(KeyCode.S))
                    direction += Vector3.back;

                if (Input.GetKey(KeyCode.A))
                    direction += Vector3.left;

                if (Input.GetKey(KeyCode.D))
                    direction += Vector3.right;

                return direction * _cameraConfig.MoveSpeed * Time.deltaTime;
            }
        }

        private bool IsPointerOverUI()
        {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null)
                return false;

            return eventSystem.IsPointerOverGameObject();
        }
    }
}
