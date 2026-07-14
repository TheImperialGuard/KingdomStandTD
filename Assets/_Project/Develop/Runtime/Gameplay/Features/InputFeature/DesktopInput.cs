using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const KeyCode RayShooterKeyCode = KeyCode.Mouse0;

        public bool IsEnabled { get; set; } = true;

        public bool RayShotRequested => Input.GetKeyDown(RayShooterKeyCode);

        public Ray CameraRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
