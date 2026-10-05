using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Предпросмотр вставки уровня из буфера обмена: изменения данных, анализ после правки, ошибки и предупреждения.
    // Окно модальное — пока оно открыто, уровень не меняется. Применить можно, только если ошибок нет.
    public class LevelPasteDialog : EditorWindow
    {
        private const string WINDOW_TITLE = "Вставка уровня из буфера";
        private const string STYLE_FILE_NAME = "LevelEditorWindow.uss";
        private const float WINDOW_WIDTH = 640f;
        private const float WINDOW_HEIGHT = 480f;

        private const string ROOT_CLASS = "paste-dialog";
        private const string HEADER_CLASS = "paste-dialog__header";
        private const string CONTENT_CLASS = "paste-dialog__content";
        private const string SECTION_TITLE_CLASS = "paste-dialog__section-title";
        private const string LINE_CLASS = "paste-dialog__line";
        private const string ERROR_LINE_CLASS = "paste-dialog__line--error";
        private const string WARNING_LINE_CLASS = "paste-dialog__line--warning";
        private const string FOOTER_CLASS = "paste-dialog__footer";
        private const string BUTTONS_CLASS = "paste-dialog__buttons";
        private const string BUTTON_CLASS = "paste-dialog__button";

        private LevelSliceImportResult _preview;
        private string _sourceLevelName;
        private Action _confirmed;

        // Показывает предпросмотр и ждёт решения; true — вставку подтвердили.
        public static bool Confirm(LevelSliceImportResult preview, string sourceLevelName)
        {
            bool isConfirmed = false;
            LevelPasteDialog dialog = CreateInstance<LevelPasteDialog>();

            dialog._preview = preview;
            dialog._sourceLevelName = sourceLevelName;
            dialog._confirmed = () => isConfirmed = true;
            dialog.titleContent = new GUIContent(WINDOW_TITLE);
            dialog.position = GetCenteredRect();
            dialog.ShowModalUtility();

            return isConfirmed;
        }

        private static Rect GetCenteredRect()
        {
            Rect rect = new(Vector2.zero, new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT));
            rect.center = EditorGUIUtility.GetMainWindowPosition().center;

            return rect;
        }

        private static void AddSection(VisualElement content, string title, IReadOnlyCollection<string> lines, string lineClass = null)
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

        private static Button CreateButton(string text, Action clicked)
        {
            Button button = new(clicked)
            {
                text = text,
            };

            button.AddToClassList(BUTTON_CLASS);

            return button;
        }

        private void CreateGUI()
        {
            if (_preview == null)
                return;

            StyleSheet styleSheet = LevelEditorWindowFiles.LoadNextToScript<StyleSheet>(this, STYLE_FILE_NAME);

            if (styleSheet != null)
                rootVisualElement.styleSheets.Add(styleSheet);

            rootVisualElement.AddToClassList(ROOT_CLASS);

            Label header = new(DescribeSource());
            header.AddToClassList(HEADER_CLASS);

            ScrollView content = new();
            content.AddToClassList(CONTENT_CLASS);

            AddSection(content, "Изменения", _preview.Changes);
            AddSection(content, "Анализ после правки", _preview.AnalysisChanges);
            AddSection(content, "Ошибки", _preview.Errors, ERROR_LINE_CLASS);
            AddSection(content, "Предупреждения после вставки", _preview.Warnings, WARNING_LINE_CLASS);

            Label footer = new(DescribeOutcome());
            footer.AddToClassList(FOOTER_CLASS);

            Button applyButton = CreateButton("Применить", OnApplyClicked);
            applyButton.SetEnabled(_preview.HasErrors == false && _preview.HasChanges);

            VisualElement buttons = new();
            buttons.AddToClassList(BUTTONS_CLASS);
            buttons.Add(CreateButton("Отмена", Close));
            buttons.Add(applyButton);

            rootVisualElement.Add(header);
            rootVisualElement.Add(content);
            rootVisualElement.Add(footer);
            rootVisualElement.Add(buttons);
        }

        private string DescribeSource()
        {
            string targetLevelName = _preview.LevelName;

            if (_sourceLevelName == targetLevelName)
                return $"Срез уровня {targetLevelName} из буфера обмена → в этот же уровень";

            return $"Срез уровня {_sourceLevelName} из буфера обмена → в уровень {targetLevelName}";
        }

        private string DescribeOutcome()
        {
            if (_preview.HasErrors)
                return "Вставить нельзя: исправьте ошибки в срезе и скопируйте его снова.";

            return "Заменятся стартовое золото, здоровье игрока и все стейджи. Вставка — один шаг Undo, "
                + "уровень останется несохранённым.";
        }

        private void OnApplyClicked()
        {
            _confirmed?.Invoke();
            Close();
        }
    }
}
