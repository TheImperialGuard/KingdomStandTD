using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    // Масштаб канваса меню (Р-78 меню): CanvasScaler в режиме Expand, на телефоне эталон меньше, и интерфейс крупнее.
    // Телефон — логическая высота игровой области ниже порога: в браузере в CSS-точках, в редакторе в пикселях окна Game.
    public static class MenuCanvasScale
    {
        public const int PhoneHeightThreshold = 480;

        public static readonly Vector2 DefaultReference = new(1440f, 1080f);
        public static readonly Vector2 PhoneReference = new(1280f, 960f);

        public static bool IsPhone(int logicalHeight) => logicalHeight < PhoneHeightThreshold;

        public static Vector2 GetReference(bool isPhone) => isPhone ? PhoneReference : DefaultReference;

        // Expand: экран вмещает эталон целиком, масштаб — меньшее из отношений сторон.
        public static float Calculate(Vector2 screenSize, bool isPhone)
        {
            Vector2 reference = GetReference(isPhone);

            return Mathf.Min(screenSize.x / reference.x, screenSize.y / reference.y);
        }
    }
}
