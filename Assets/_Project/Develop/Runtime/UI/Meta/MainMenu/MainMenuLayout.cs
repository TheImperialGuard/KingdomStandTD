using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    // Раскладка главного меню по формулам макета (D.html → layout()). Числа — в px макета: в единицах эталона канваса они
    // в MockupToReference раз больше. Вход — размер safe area: формулы макета вычитают боковой отступ с обеих сторон.
    // Выход — в единицах эталона.
    public class MainMenuLayout
    {
        public const float MockupToReference = 1.5f;

        private const float MenuMargin = 48f;
        private const float ExpandedMenuWidth = 288f;
        private const float CollapsedMenuWidth = 72f;
        private const float BandTail = 64f;
        private const float PanelGap = 24f;
        private const float PanelMargin = 40f;
        private const float MinPanelRoom = 600f;
        private const float MaxPanelWidth = 700f;
        private const float HighPanelTop = 84f;
        private const float LowPanelTop = 60f;
        private const float MinHeightForHighPanelTop = 700f;

        private MainMenuLayout(bool isMenuCollapsed, float panelWidth, float panelTop)
        {
            IsMenuCollapsed = isMenuCollapsed;
            PanelWidth = panelWidth;
            PanelTop = panelTop;
        }

        public bool IsMenuCollapsed { get; }

        public float PanelWidth { get; }

        // Верх панели — от верха экрана: цепи доски висят с его края.
        public float PanelTop { get; }

        // Правый край панели — на этом расстоянии от правого края safe area.
        public float PanelRightMargin => PanelMargin * MockupToReference;

        // Высота экрана в макете — полная, без safe area: на телефоне эталон ниже, и доска поднимается.
        public static MainMenuLayout Calculate(Vector2 areaSize, float screenHeight, MainMenuPanels openedPanel)
        {
            float areaWidth = areaSize.x / MockupToReference;

            bool isMenuCollapsed = openedPanel != MainMenuPanels.None
                && GetPanelRoom(areaWidth, ExpandedMenuWidth) < MinPanelRoom;

            float menuWidth = isMenuCollapsed ? CollapsedMenuWidth : ExpandedMenuWidth;
            float panelWidth = Mathf.Min(MaxPanelWidth, GetPanelRoom(areaWidth, menuWidth));
            float panelTop = screenHeight / MockupToReference >= MinHeightForHighPanelTop ? HighPanelTop : LowPanelTop;

            return new MainMenuLayout(
                isMenuCollapsed,
                panelWidth * MockupToReference,
                panelTop * MockupToReference);
        }

        // Место под панель — от конца полосы меню с зазором до правого поля области.
        private static float GetPanelRoom(float areaWidth, float menuWidth)
            => areaWidth - PanelMargin - (MenuMargin + menuWidth + BandTail + PanelGap);
    }
}
