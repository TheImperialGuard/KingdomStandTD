using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    // Свет уровня со своим окружением, например снежным: солнце сцены и подсветка окружения.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/LevelLightingConfig", fileName = "LevelLightingConfig")]
    public class LevelLightingConfig : ScriptableObject
    {
        [field: SerializeField] public Color SunColor { get; private set; } = Color.white;

        [field: SerializeField, Min(0f)] public float SunIntensity { get; private set; } = 1f;

        [field: Tooltip("Поворот солнца в градусах: по X — высота над горизонтом, по Y — направление")]
        [field: SerializeField] public Vector3 SunRotation { get; private set; } = new Vector3(60f, 330f, 0f);

        [field: Tooltip("Сила теней от солнца: 0 — теней нет, 1 — самые тёмные")]
        [field: SerializeField, Range(0f, 1f)] public float SunShadowStrength { get; private set; } = 1f;

        [field: Header("Подсветка окружения — градиент: сверху, с горизонта, снизу")]
        [field: SerializeField] public Color AmbientSkyColor { get; private set; } = new Color(0.212f, 0.227f, 0.259f);

        [field: SerializeField] public Color AmbientEquatorColor { get; private set; } = new Color(0.114f, 0.125f, 0.133f);

        [field: SerializeField] public Color AmbientGroundColor { get; private set; } = new Color(0.047f, 0.043f, 0.035f);
    }
}
