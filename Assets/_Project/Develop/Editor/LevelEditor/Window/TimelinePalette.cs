using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Цвета графиков окна — таймлайна и кривой сложности — для светлой и тёмной темы редактора. Цвета врагов —
    // проверенная категориальная палитра скилла dataviz: восемь оттенков в постоянном порядке, для тёмной темы —
    // свои шаги тех же оттенков. Подложка и линии — из той же палитры: на этой подложке цвета и проверены.
    public static class TimelinePalette
    {
        public const int NO_SLOT = -1;

        // Подложка полоски волны с одним видом врагов — цвет врага с такой прозрачностью.
        private const float ENEMY_TRACK_ALPHA = 0.25f;

        // Всё, что не подсвечено, — приглушённый серый с такой непрозрачностью.
        private const float DIMMED_ALPHA = 0.35f;

        // График с одной серией берёт первый цвет палитры.
        private const int MAIN_SERIES_SLOT = 0;

        private static readonly Color[] _lightEnemyColors =
        {
            Hex("#2a78d6"),
            Hex("#eb6834"),
            Hex("#1baf7a"),
            Hex("#eda100"),
            Hex("#e87ba4"),
            Hex("#008300"),
            Hex("#4a3aa7"),
            Hex("#e34948"),
        };

        private static readonly Color[] _darkEnemyColors =
        {
            Hex("#3987e5"),
            Hex("#d95926"),
            Hex("#199e70"),
            Hex("#c98500"),
            Hex("#d55181"),
            Hex("#008300"),
            Hex("#9085e9"),
            Hex("#e66767"),
        };

        private static readonly Color _lightSurface = Hex("#fcfcfb");
        private static readonly Color _darkSurface = Hex("#1a1a19");
        private static readonly Color _lightGridline = Hex("#e1e0d9");
        private static readonly Color _darkGridline = Hex("#2c2c2a");
        private static readonly Color _lightAxis = Hex("#c3c2b7");
        private static readonly Color _darkAxis = Hex("#383835");
        private static readonly Color _lightText = Hex("#52514e");
        private static readonly Color _darkText = Hex("#c3c2b7");
        private static readonly Color _lightInk = Hex("#0b0b0b");
        private static readonly Color _darkInk = Hex("#ffffff");
        private static readonly Color _mutedInk = Hex("#898781");

        // Как --color-warning в LevelEditorWindow.uss: так же окно помечает спавн после конца стейджа.
        private static readonly Color _warning = Hex("#E8A33D");

        private static readonly Color _lightWash = new(0f, 0f, 0f, 0.04f);
        private static readonly Color _darkWash = new(1f, 1f, 1f, 0.04f);
        private static readonly Color _lightStrongWash = new(0f, 0f, 0f, 0.09f);
        private static readonly Color _darkStrongWash = new(1f, 1f, 1f, 0.09f);

        public static int EnemySlotsCount => _lightEnemyColors.Length;

        public static Color Surface => IsDark ? _darkSurface : _lightSurface;
        public static Color Gridline => IsDark ? _darkGridline : _lightGridline;
        public static Color Axis => IsDark ? _darkAxis : _lightAxis;
        public static Color Text => IsDark ? _darkText : _lightText;
        public static Color MutedText => _mutedInk;
        public static Color StageBoundary => _mutedInk;
        public static Color Selection => IsDark ? _darkInk : _lightInk;
        public static Color Hover => IsDark ? _darkText : _lightText;
        public static Color Warning => _warning;
        public static Color MainSeries => GetEnemyColor(MAIN_SERIES_SLOT);

        // Засечки других врагов при подсветке врага, столбики других уровней при выборе уровня.
        public static Color Dimmed => WithAlpha(_mutedInk, DIMMED_ALPHA);

        // Лёгкая заливка: чередование стейджей, время после конца стейджа, нейтральная подложка волны.
        public static Color Wash => IsDark ? _darkWash : _lightWash;

        // Заливка заметнее: выбранный стейдж.
        public static Color StrongWash => IsDark ? _darkStrongWash : _lightStrongWash;

        private static bool IsDark => EditorGUIUtility.isProSkin;

        // Врагам вне палитры («прочие») и группам без врага — приглушённый серый.
        public static Color GetEnemyColor(int slot)
        {
            Color[] colors = IsDark ? _darkEnemyColors : _lightEnemyColors;

            return slot >= 0 && slot < colors.Length ? colors[slot] : _mutedInk;
        }

        // Подложка волны с одним видом врагов. У «прочих» она нейтральная, как у смешанной волны.
        public static Color GetTrackColor(int slot) => slot == NO_SLOT ? Wash : GetEnemyTrackColor(slot);

        // Подложка в цвете врага, у «прочих» — серая: так выделяются волны подсвеченного врага.
        public static Color GetEnemyTrackColor(int slot) => WithAlpha(GetEnemyColor(slot), ENEMY_TRACK_ALPHA);

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);

            return color;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;

            return color;
        }
    }
}
