using System;
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

        // Окно можно растягивать; меньше этого заголовок и кнопки перестают помещаться.
        private const float MIN_WINDOW_WIDTH = 400f;
        private const float MIN_WINDOW_HEIGHT = 240f;

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
            dialog.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
            dialog.position = DialogElements.GetCenteredRect(WINDOW_WIDTH, WINDOW_HEIGHT);
            dialog.ShowModalUtility();

            return isConfirmed;
        }

        private void CreateGUI()
        {
            if (_preview == null)
                return;

            StyleSheet styleSheet = LevelEditorWindowFiles.LoadNextToScript<StyleSheet>(this, STYLE_FILE_NAME);

            if (styleSheet != null)
                rootVisualElement.styleSheets.Add(styleSheet);

            rootVisualElement.AddToClassList(DialogElements.ROOT_CLASS);

            Label header = new(DescribeSource());
            header.AddToClassList(DialogElements.HEADER_CLASS);

            ScrollView content = new();
            content.AddToClassList(DialogElements.CONTENT_CLASS);

            DialogElements.AddSection(content, "Изменения", _preview.Changes);
            DialogElements.AddSection(content, "Анализ после правки", _preview.AnalysisChanges);
            DialogElements.AddSection(content, "Ошибки", _preview.Errors, DialogElements.ERROR_LINE_CLASS);
            DialogElements.AddSection(content, "Предупреждения после вставки", _preview.Warnings, DialogElements.WARNING_LINE_CLASS);

            Label footer = new(DescribeOutcome());
            footer.AddToClassList(DialogElements.FOOTER_CLASS);

            Button applyButton = DialogElements.CreateButton("Применить", OnApplyClicked);
            applyButton.SetEnabled(_preview.HasErrors == false && _preview.HasChanges);

            VisualElement buttons = new();
            buttons.AddToClassList(DialogElements.BUTTONS_CLASS);
            buttons.Add(DialogElements.CreateButton("Отмена", Close));
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
