using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Высота строки текста TMP как CSS line-height в долях кегля. TMP считает высоту блока от верха букв первой строки до
    // низа последней, и блок выходит выше CSS-блока на (высота строки шрифта − line-height) кегля. Компонент ставит шаг
    // строк и отдаёт группам раскладки высоту CSS-блока; буквы при вертикальном выравнивании Middle выступают за него
    // поровну сверху и снизу, как в CSS.
    [RequireComponent(typeof(TMP_Text))]
    public class TextLineHeight : MonoBehaviour, ILayoutElement
    {
        private const float LineSpacingUnitsPerEm = 100f;
        private const int OverTextLayoutPriority = 1;

        [SerializeField] private TMP_Text _text;
        [SerializeField, Min(0f)] private float _lineHeight = 1.2f;

        public float minWidth => -1f;

        public float preferredWidth => -1f;

        public float flexibleWidth => -1f;

        public float minHeight => -1f;

        public float preferredHeight => _text.preferredHeight - GetExcessHeight();

        public float flexibleHeight => -1f;

        public int layoutPriority => OverTextLayoutPriority;

        private float FontLineHeight => _text.font.faceInfo.lineHeight / _text.font.faceInfo.pointSize;

        private void Awake() => ApplyLineSpacing();

        private void OnValidate()
        {
            if (_text == null)
                _text = GetComponent<TMP_Text>();

            if (_text.font != null)
                ApplyLineSpacing();
        }

        public void CalculateLayoutInputHorizontal() { }

        public void CalculateLayoutInputVertical() { }

        private void ApplyLineSpacing() => _text.lineSpacing = (_lineHeight - FontLineHeight) * LineSpacingUnitsPerEm;

        private float GetExcessHeight() => (FontLineHeight - _lineHeight) * _text.fontSize;
    }
}
