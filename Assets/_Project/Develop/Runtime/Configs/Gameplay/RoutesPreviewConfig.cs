using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/RoutesPreviewConfig", fileName = "RoutesPreviewConfig")]
    public class RoutesPreviewConfig : ScriptableObject
    {
        [SerializeField] private List<RoutePreviewColor> _pathsColors = new()
        {
            new RoutePreviewColor(PathNumber.First, new Color(1f, 0.35f, 0.15f, 0.85f)),
            new RoutePreviewColor(PathNumber.Second, new Color(0.25f, 0.6f, 1f, 0.85f)),
            new RoutePreviewColor(PathNumber.Third, new Color(0.4f, 1f, 0.3f, 0.85f)),
        };

        [field: SerializeField] public string PrefabPath { get; private set; }

        [field: Tooltip("Период запуска новых цепочек стрелок на каждом пути")]
        [field: SerializeField, Min(0.1f)] public float SpawnInterval { get; private set; } = 1.5f;

        [field: Tooltip("Расстояние между соседними стрелками цепочки. На каждом пути оно чуть уменьшается, " +
            "чтобы последняя стрелка встала ровно на последний вейпоинт")]
        [field: SerializeField, Min(0.1f)] public float StepDistance { get; private set; } = 0.8f;

        [field: Tooltip("Время жизни стрелки: за сколько секунд она проходит путь от появления до полного исчезновения")]
        [field: SerializeField, Min(0.05f)] public float FadeDuration { get; private set; } = 0.6f;

        [field: Tooltip("На какой доле затухания стрелки впереди неё появляется следующая")]
        [field: SerializeField, Range(0.05f, 1f)] public float NextStepFadeProgress { get; private set; } = 0.33f;

        [field: Tooltip("Сколько длится разгорание стрелки, в долях шага. 1 — разгорание на весь шаг: " +
            "яркость цепочки ровная, без вспышек. 0 — стрелка появляется сразу яркой")]
        [field: SerializeField, Range(0f, 1f)] public float AppearStepFraction { get; private set; } = 1f;

        [field: Tooltip("Размер стрелки за время её жизни: по горизонтали 0 — появление, 1 — исчезновение, " +
            "по вертикали множитель к масштабу префаба")]
        [field: SerializeField] public AnimationCurve ScaleOverLifetime { get; private set; } = new(
            new Keyframe(0f, 0.75f),
            new Keyframe(0.15f, 1f),
            new Keyframe(1f, 0.6f));

        [field: Tooltip("Подъём стрелки над вейпоинтом, чтобы она не тонула в дороге")]
        [field: SerializeField] public float HeightOffset { get; private set; } = 0.05f;

        [field: Tooltip("Насколько вперёд по пути смотрит стрелка: чем больше, тем раньше и плавнее она поворачивает на углах")]
        [field: SerializeField, Min(0.01f)] public float LookAheadDistance { get; private set; } = 0.3f;

        public float StepInterval => FadeDuration * NextStepFadeProgress;
        public float AppearDuration => StepInterval * AppearStepFraction;

        public bool TryGetColor(PathNumber pathNumber, out Color color)
        {
            foreach (RoutePreviewColor pathColor in _pathsColors)
            {
                if (pathColor.PathNumber == pathNumber)
                {
                    color = pathColor.Color;
                    return true;
                }
            }

            color = default;
            return false;
        }
    }
}
