using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const KeyCode RayShooterKeyCode = KeyCode.Mouse0;

        public bool IsEnabled { get; set; } = true;

        public bool RayShotRequested 
            => Input.GetKeyDown(RayShooterKeyCode) && EventSystem.current.IsPointerOverGameObject() == false;

        public Ray CameraRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        public Vector3 CameraDelta
        {
            get
            {
                Vector3 delta = Vector3.zero;

                if (IsEnabled == false)
                    return delta;

                if (Input.GetKey(KeyCode.W)) 
                    delta += Vector3.forward;

                if (Input.GetKey(KeyCode.S)) 
                    delta += Vector3.back;

                if (Input.GetKey(KeyCode.A)) 
                    delta += Vector3.left;

                if (Input.GetKey(KeyCode.D)) 
                    delta += Vector3.right;

                // Свайпы
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 touchDelta = touch.deltaPosition;
                        delta += new Vector3(-touchDelta.x, -touchDelta.y, 0) * Time.deltaTime;
                    }
                }

                return delta;
            }
        }
    }
}
