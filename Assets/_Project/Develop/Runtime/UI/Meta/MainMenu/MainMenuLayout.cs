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
        private const float SettingsPanelWidth = 480f;
        private const float SettingsLeftGapShare = 0.55f;
        private const float RoundHalfUp = 0.5f;
        private const float BookWidth = 792f;
        private const float BookHeight = 544f;
        private const float MinBookTop = 24f;

        private MainMenuLayout(
            bool isMenuCollapsed,
            float panelWidth,
            float panelTop,
            float panelRightMargin,
            float panelScale = 1f)
        {
            IsMenuCollapsed = isMenuCollapsed;
            PanelWidth = panelWidth;
            PanelTop = panelTop;
            PanelRightMargin = panelRightMargin;
            PanelScale = panelScale;
        }

        public bool IsMenuCollapsed { get; }

        public float PanelWidth { get; }

        // Верх панели — от верха экрана: цепи доски висят с его края.
        public float PanelTop { get; }

        // Правый край панели — на этом расстоянии от правого края safe area.
        public float PanelRightMargin { get; }

        // Масштаб панели, которая не тянется, а уменьшается целиком, — книги.
        public float PanelScale { get; }

        // Высота экрана в макете — полная, без safe area: на телефоне эталон ниже, и доска поднимается.
        public static MainMenuLayout Calculate(Vector2 areaSize, float screenHeight, MainMenuPanels openedPanel)
        {
            float areaWidth = areaSize.x / MockupToReference;
            float panelTop = screenHeight / MockupToReference >= MinHeightForHighPanelTop ? HighPanelTop : LowPanelTop;

            if (openedPanel == MainMenuPanels.Settings)
                return CalculateSettings(areaWidth, panelTop);

            if (openedPanel == MainMenuPanels.Encyclopedia)
                return CalculateBook(areaWidth, areaSize.y / MockupToReference, panelTop);

            bool isMenuCollapsed = openedPanel != MainMenuPanels.None
                && GetPanelRoom(areaWidth, ExpandedMenuWidth) < MinPanelRoom;

            float menuWidth = isMenuCollapsed ? CollapsedMenuWidth : ExpandedMenuWidth;
            float panelWidth = Mathf.Min(MaxPanelWidth, GetPanelRoom(areaWidth, menuWidth));

            return new MainMenuLayout(
                isMenuCollapsed,
                panelWidth * MockupToReference,
                panelTop * MockupToReference,
                PanelMargin * MockupToReference);
        }

        // Окно настроек (spec-settings.md → «Место окна»): ширина постоянная, меню сворачивается, только если рядом с полным
        // меню не хватает места на неё; свободная ширина делится 55% слева и остаток справа.
        private static MainMenuLayout CalculateSettings(float areaWidth, float panelTop)
        {
            bool isMenuCollapsed = GetPanelRoom(areaWidth, ExpandedMenuWidth) < SettingsPanelWidth;

            float menuWidth = isMenuCollapsed ? CollapsedMenuWidth : ExpandedMenuWidth;
            float freeWidth = Mathf.Max(0f, areaWidth - (MenuMargin + menuWidth + BandTail) - SettingsPanelWidth);

            // Math.round макета округляет половину вверх, Mathf.Round — к чётному.
            float leftGap = Mathf.Floor(freeWidth * SettingsLeftGapShare + RoundHalfUp);

            return new MainMenuLayout(
                isMenuCollapsed,
                SettingsPanelWidth * MockupToReference,
                panelTop * MockupToReference,
                (freeWidth - leftGap) * MockupToReference);
        }

        // Книга с закладками (spec-encyclopedia.md → «Телефон и планшет», layout() в make_book.py): меню сворачивается, если
        // книга не помещается рядом с полным меню; не помещается и рядом с иконками — уменьшается до свободного места.
        // Высота — safe area: макет ставит книгу по центру между верхом экрана и safe area снизу, а верхнего отступа
        // в альбомной ориентации нет.
        private static MainMenuLayout CalculateBook(float areaWidth, float areaHeight, float panelTop)
        {
            bool isMenuCollapsed = GetPanelRoom(areaWidth, ExpandedMenuWidth) < BookWidth;

            float menuWidth = isMenuCollapsed ? CollapsedMenuWidth : ExpandedMenuWidth;
            float widthScale = GetPanelRoom(areaWidth, menuWidth) / BookWidth;
            float heightScale = (areaHeight - 2f * MinBookTop) / BookHeight;
            float scale = Mathf.Min(1f, widthScale, heightScale);

            return new MainMenuLayout(
                isMenuCollapsed,
                BookWidth * scale * MockupToReference,
                panelTop * MockupToReference,
                PanelMargin * MockupToReference,
                scale);
        }

        // Место под панель — от конца полосы меню с зазором до правого поля области.
        private static float GetPanelRoom(float areaWidth, float menuWidth)
            => areaWidth - PanelMargin - (MenuMargin + menuWidth + BandTail + PanelGap);
    }
}
