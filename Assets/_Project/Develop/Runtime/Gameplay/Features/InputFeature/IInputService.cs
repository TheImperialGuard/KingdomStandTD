using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public interface IInputService
    {
        bool IsEnabled { get; set; }

        bool RayShotRequested { get; }

        Ray CameraRay { get; }

        /// <summary>
        /// Готовое смещение камеры в мировых координатах за текущий кадр.
        /// Реализация сама учитывает скорость и время кадра, потребитель применяет значение как есть.
        /// </summary>
        Vector3 CameraDelta { get; }
    }
}
