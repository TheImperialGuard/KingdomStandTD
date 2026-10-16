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

        /// <summary>
        /// Просьба закрыть открытую панель интерфейса. Не зависит от IsEnabled: тот выключает игровой ввод,
        /// а панель должна закрываться и тогда.
        /// </summary>
        bool CloseRequested { get; }

        /// <summary>
        /// Просьбы листать назад и вперёд — например, развороты книги. Как и CloseRequested, не зависят от IsEnabled.
        /// </summary>
        bool PreviousRequested { get; }

        bool NextRequested { get; }
    }
}
