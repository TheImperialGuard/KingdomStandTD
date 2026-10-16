using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Правая страница заметки: текст с буквицей цвета раздела. Буквица стоит в первой строке, как ::first-letter макета, —
    // rich text TMP. Абзацы — переводы строки в тексте, промежуток между ними — paragraphSpacing компонента.
    public class EncyclopediaNotePageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_FontAsset _initialFont;
        // Целые: дробное число в теге записалось бы через запятую русской локали.
        [SerializeField, Min(0)] private int _initialSize = 69;
        [SerializeField, Min(0)] private int _initialGap = 12;

        private string _initialColor;

        // Тег <font> ищет шрифт по имени среди зарегистрированных: шрифт буквицы лежит не в Resources TMP.
        private void Awake() => MaterialReferenceManager.AddFontAsset(_initialFont);

        public void SetAccentColor(Color accentColor) => _initialColor = ColorUtility.ToHtmlStringRGB(accentColor);

        public void SetText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _text.text = string.Empty;
                return;
            }

            string initial = text.Substring(0, 1);
            string rest = text.Substring(1);
            string fontName = _initialFont.name;

            _text.text = $"<font=\"{fontName}\"><size={_initialSize}><color=#{_initialColor}>{initial}</color></size></font>"
                + $"<space={_initialGap}>{rest}";
        }
    }
}
