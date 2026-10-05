using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Общие части модальных окон редактора уровней — вставки уровня и массовых операций: разделы предпросмотра,
    // кнопки и положение окна по центру редактора. Стили — в LevelEditorWindow.uss, классы dialog__…
    public static class DialogElements
    {
        public const string ROOT_CLASS = "dialog";
        public const string HEADER_CLASS = "dialog__header";
        public const string CONTENT_CLASS = "dialog__content";
        public const string FOOTER_CLASS = "dialog__footer";
        public const string BUTTONS_CLASS = "dialog__buttons";
        public const string ERROR_LINE_CLASS = "dialog__line--error";
        public const string WARNING_LINE_CLASS = "dialog__line--warning";

        private const string SECTION_TITLE_CLASS = "dialog__section-title";
        private const string LINE_CLASS = "dialog__line";
        private const string BUTTON_CLASS = "dialog__button";

        public static Rect GetCenteredRect(float width, float height)
        {
            Rect rect = new(Vector2.zero, new Vector2(width, height));
            rect.center = EditorGUIUtility.GetMainWindowPosition().center;

            return rect;
        }

        // Раздел предпросмотра: заголовок с числом строк и сами строки; пустой раздел не показывается.
        public static void AddSection(VisualElement content, string title, IReadOnlyCollection<string> lines, string lineClass = null)
        {
            if (lines.Count == 0)
                return;

            Label titleLabel = new($"{title} ({lines.Count})");
            titleLabel.AddToClassList(SECTION_TITLE_CLASS);
            content.Add(titleLabel);

            foreach (string line in lines)
            {
                Label lineLabel = new(line);
                lineLabel.AddToClassList(LINE_CLASS);

                if (lineClass != null)
                    lineLabel.AddToClassList(lineClass);

                content.Add(lineLabel);
            }
        }

        public static Button CreateButton(string text, Action clicked)
        {
            Button button = new(clicked)
            {
                text = text,
            };

            button.AddToClassList(BUTTON_CLASS);

            return button;
        }
    }
}
