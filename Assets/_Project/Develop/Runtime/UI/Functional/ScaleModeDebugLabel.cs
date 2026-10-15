using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    // Временная отладочная надпись с логической высотой и режимом масштаба интерфейса. Рисуется только в редакторе
    // и в Development Build, в обычной сборке компонент пустой. Текст латиницей: встроенный шрифт IMGUI в WebGL
    // может не содержать кириллицы.
    [RequireComponent(typeof(PhoneScaleSwitcher))]
    public class ScaleModeDebugLabel : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const float FontSizeToScreenHeight = 0.035f;
        private const float MarginToScreenHeight = 0.015f;
        private const string PhoneModeName = "Phone mode";
        private const string DefaultModeName = "Default mode";

        private readonly GUIContent _content = new();

        private PhoneScaleSwitcher _phoneScaleSwitcher;
        private GUIStyle _style;

        private void Awake()
        {
            _phoneScaleSwitcher = GetComponent<PhoneScaleSwitcher>();
        }

        private void OnGUI()
        {
            if (_style == null)
                _style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft };

            int screenHeight = Screen.height;
            _style.fontSize = Mathf.RoundToInt(screenHeight * FontSizeToScreenHeight);

            string modeName = _phoneScaleSwitcher.IsPhone ? PhoneModeName : DefaultModeName;
            _content.text = $"Logical height {_phoneScaleSwitcher.LogicalHeight} (screen {screenHeight}) | {modeName}";

            float margin = screenHeight * MarginToScreenHeight;
            Vector2 size = _style.CalcSize(_content);
            GUI.Box(new Rect(margin, margin, size.x, size.y), _content, _style);
        }
#endif
    }
}
