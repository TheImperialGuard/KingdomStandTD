using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    // Раскладка главного меню по формулам макета (D.html → layout()). Числа — в px макета: в единицах эталона канваса они
    // в MockupToReference раз больше. Вход — размер safe area: формулы макета вычитают боковой отступ с обеих сторон.
    public class MainMenuLayout
    {
        private const float MockupToReference = 1.5f;
        private const float MenuMargin = 48f;
        private const float ExpandedMenuWidth = 288f;
        private const float BandTail = 64f;
        private const float PanelGap = 24f;
        private const float PanelMargin = 40f;
        private const float MinPanelRoom = 600f;

        private MainMenuLayout(bool isMenuCollapsed)
        {
            IsMenuCollapsed = isMenuCollapsed;
        }

        public bool IsMenuCollapsed { get; }

        public static MainMenuLayout Calculate(Vector2 areaSize, MainMenuPanels openedPanel)
        {
            float areaWidth = areaSize.x / MockupToReference;

            bool isMenuCollapsed = openedPanel != MainMenuPanels.None
                && GetPanelRoom(areaWidth, ExpandedMenuWidth) < MinPanelRoom;

            return new MainMenuLayout(isMenuCollapsed);
        }

        // Место под панель — от конца полосы меню с зазором до правого поля области.
        private static float GetPanelRoom(float areaWidth, float menuWidth)
            => areaWidth - PanelMargin - (MenuMargin + menuWidth + BandTail + PanelGap);
    }
}
