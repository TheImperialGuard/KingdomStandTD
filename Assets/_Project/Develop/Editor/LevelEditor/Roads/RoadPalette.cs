using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Roads
{
    // Цвета дорог в Scene View. Дороги — как --color-road-* в LevelEditorWindow.uss: дорога в таблице волн и в сцене
    // одного цвета. Подписи не красятся в цвет дороги: текст белый, а тёмная обводка и подложка отделяют линии
    // и текст от земли уровня.
    public static class RoadPalette
    {
        private static readonly Color _firstRoad = new Color32(0x4C, 0x9A, 0xFF, 0xFF);
        private static readonly Color _secondRoad = new Color32(0xFF, 0x9F, 0x43, 0xFF);
        private static readonly Color _thirdRoad = new Color32(0x2E, 0xCC, 0x71, 0xFF);

        // Как приглушённый серый TimelinePalette: дорога с номером вне палитры.
        private static readonly Color _otherRoad = new Color32(0x89, 0x87, 0x81, 0xFF);

        public static Color Outline { get; } = new(0f, 0f, 0f, 0.75f);
        public static Color LabelBackground { get; } = new(0f, 0f, 0f, 0.65f);
        public static Color Text { get; } = Color.white;
        public static Color TextShadow { get; } = new(0f, 0f, 0f, 0.9f);
        public static Color Base { get; } = new(1f, 1f, 1f, 0.9f);

        public static Color GetRoadColor(PathNumber number)
        {
            switch (number)
            {
                case PathNumber.First:
                    return _firstRoad;

                case PathNumber.Second:
                    return _secondRoad;

                case PathNumber.Third:
                    return _thirdRoad;

                default:
                    return _otherRoad;
            }
        }
    }
}
