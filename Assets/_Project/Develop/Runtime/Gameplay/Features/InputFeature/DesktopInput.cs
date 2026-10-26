using Assets._Project.Develop.Runtime.Configs.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const KeyCode RAY_SHOOTER_KEY_CODE = KeyCode.Mouse0;
        private const KeyCode CloseKeyCode = KeyCode.Escape;
        private const KeyCode PreviousKeyCode = KeyCode.LeftArrow;
        private const KeyCode NextKeyCode = KeyCode.RightArrow;
        private const float NoZoom = 1f;

        // Браузеры отдают прокрутку по-разному, и быстрый прокрут может прийти одним большим значением:
        // больше одного щелчка за кадр не засчитываем.
        private const float MaxWheelStepsPerFrame = 1f;

        private readonly CameraConfig _cameraConfig;

        public DesktopInput(CameraConfig cameraConfig)
        {
            _cameraConfig = cameraConfig;
        }

        public bool IsEnabled { get; set; } = true;

        public bool RayShotRequested
            => IsEnabled && Input.GetKeyDown(RAY_SHOOTER_KEY_CODE) && IsPointerOverUI() == false;

        public Ray CameraRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        public bool CloseRequested => Input.GetKeyDown(CloseKeyCode);

        public bool PreviousRequested => Input.GetKeyDown(PreviousKeyCode);

        public bool NextRequested => Input.GetKeyDown(NextKeyCode);

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

                // Скорость задана в высотах экрана: при любом масштабе карта бежит по экрану одинаково.
                float screenHeight = Camera.main.orthographicSize * 2f;

                return direction * _cameraConfig.MoveSpeed * screenHeight * Time.deltaTime;
            }
        }

        public float CameraZoomFactor
        {
            get
            {
                if (IsEnabled == false)
                    return NoZoom;

                float wheelSteps = Mathf.Clamp(Input.mouseScrollDelta.y, -MaxWheelStepsPerFrame, MaxWheelStepsPerFrame);

                if (wheelSteps == 0f || IsPointerOverUI())
                    return NoZoom;

                // Колёсико от себя приближает: масштаб камеры уменьшается.
                return Mathf.Pow(_cameraConfig.WheelZoomStep, -wheelSteps);
            }
        }

        public Vector2 CameraZoomPivot => Input.mousePosition;

        private bool IsPointerOverUI()
        {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null)
                return false;

            return eventSystem.IsPointerOverGameObject();
        }
    }
}
