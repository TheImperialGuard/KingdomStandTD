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
        /// Во сколько раз изменить масштаб камеры за текущий кадр: меньше 1 — приблизить, больше 1 — отдалить, 1 — не менять.
        /// </summary>
        float CameraZoomFactor { get; }

        /// <summary>
        /// Точка экрана, вокруг которой меняется масштаб: точка поля под ней остаётся на месте.
        /// </summary>
        Vector2 CameraZoomPivot { get; }

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
