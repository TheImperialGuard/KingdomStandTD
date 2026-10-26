using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/CameraConfig", fileName = "CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [field: Tooltip("Скорость камеры с клавиатуры, в высотах экрана в секунду: по экрану карта бежит одинаково " +
            "при любом масштабе")]
        [field: SerializeField, Min(0.01f)] public float MoveSpeed { get; private set; } = 0.8f;

        [field: Tooltip("Во сколько раз камеру можно приблизить относительно начального вида уровня. 1 — без зума")]
        [field: SerializeField, Min(1f)] public float MaxZoom { get; private set; } = 2f;

        [field: Tooltip("Во сколько раз меняется масштаб за один щелчок колёсика мыши")]
        [field: SerializeField, Min(1.01f)] public float WheelZoomStep { get; private set; } = 1.15f;
    }
}
