using Assets._Project.Develop.Runtime.UI.Functional;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.PausePopup
{
    // Раскладка паузы (spec-pause.md → «Раскладка», «Масштаб под экран»; layout() в make_pause.py). Кадры паузы собраны в
    // масштабе меню (Р-06), поэтому расчёт идёт в px макета меню: доска по центру safe area, верх по высоте экрана, при
    // нехватке высоты окно сжимается на k вокруг верха доски. Выход — для корня доски, который висит от верха экрана над
    // центром доски: масштаб, сдвиг центра от центра экрана в единицах канваса боя и верх доски в единицах эталона внутри
    // корня. Верх делится на k, чтобы после сжатия доска стояла на прежней высоте, а цепи доставали до края экрана.
    public class PausePopupLayout
    {
        public const float MockupToReference = 1.5f;
        public const float PanelWidth = MockupPanelWidth * MockupToReference;

        private const float MockupPanelWidth = 480f;
        private const float BoardHeight = 572f;
        private const float BottomGap = 8f;
        private const float HighPanelTop = 84f;
        private const float LowPanelTop = 60f;
        private const float MinHeightForHighPanelTop = 700f;
        private const float MaxFitScale = 1f;
        private const float RoundHalfUp = 0.5f;

        private PausePopupLayout(float scale, float centerOffset, float panelTop)
        {
            Scale = scale;
            CenterOffset = centerOffset;
            PanelTop = panelTop;
        }

        // Масштаб корня доски: отношение масштабов канвасов меню и боя, умноженное на k.
        public float Scale { get; }

        // Сдвиг центра доски от центра экрана по горизонтали — в единицах канваса боя.
        public float CenterOffset { get; }

        // Верх доски от верха экрана — в единицах эталона внутри корня доски.
        public float PanelTop { get; }

        // Размеры экрана и safe area — в пикселях, battleScale — scaleFactor канваса боя.
        public static PausePopupLayout Calculate(Vector2 screenSize, Rect safeArea, float battleScale, bool isPhone)
        {
            float menuScale = MenuCanvasScale.Calculate(screenSize, isPhone);
            float mockupPixel = menuScale * MockupToReference;

            float screenWidth = screenSize.x / mockupPixel;
            float screenHeight = screenSize.y / mockupPixel;
            float areaLeft = safeArea.xMin / mockupPixel;
            float areaRight = safeArea.xMax / mockupPixel;
            float areaBottom = screenHeight - safeArea.yMin / mockupPixel;

            float panelTop = screenHeight >= MinHeightForHighPanelTop ? HighPanelTop : LowPanelTop;
            float fitScale = Mathf.Min(MaxFitScale, (areaBottom - panelTop - BottomGap) / BoardHeight);

            // Math.round макета округляет половину вверх, Mathf.Round — к чётному.
            float panelLeft = areaLeft + Mathf.Floor((areaRight - areaLeft - MockupPanelWidth) / 2f + RoundHalfUp);
            float centerOffset = panelLeft + MockupPanelWidth / 2f - screenWidth / 2f;

            float menuToBattle = menuScale / battleScale;

            return new PausePopupLayout(
                menuToBattle * fitScale,
                centerOffset * MockupToReference * menuToBattle,
                panelTop * MockupToReference / fitScale);
        }
    }
}
