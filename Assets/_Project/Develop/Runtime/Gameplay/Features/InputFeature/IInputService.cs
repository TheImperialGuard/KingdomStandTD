using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public interface IInputService
    {
        bool IsEnabled { get; set; }

        bool RayShotRequested { get; }

        Ray CameraRay { get; }
    }
}
