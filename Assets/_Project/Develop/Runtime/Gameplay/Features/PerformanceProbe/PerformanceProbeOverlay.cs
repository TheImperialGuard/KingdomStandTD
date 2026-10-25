#if KS_PERF_PROBE
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PerformanceProbe
{
    // Панель замера поверх игры на IMGUI — без префабов и шрифтов проекта. Надписи латиницей: встроенному шрифту IMGUI
    // на вебе неоткуда взять недостающие символы. Касание панели может дойти и до игры — IMGUI ввод не перекрывает
    public class PerformanceProbeOverlay : MonoBehaviour
    {
        private const float ReferenceScreenSize = 1080f;
        private const float PanelLeft = 16f;
        private const float PanelTop = 200f;
        private const float PanelWidth = 720f;
        private const float PanelHeight = 480f;
        private const float PanelPadding = 12f;
        private const float CollapsedButtonWidth = 120f;
        private const float ButtonHeight = 60f;
        private const int FontSize = 24;
        private const string CollapsedButtonText = "PERF";
        private const string HideButtonText = "Hide";
        private const string RunButtonText = "Run";
        private const string WaitingText = "...";

        private static readonly Color _backgroundColor = new(0, 0, 0, 0.75f);

        public event Action RunClicked;

        private string _counterText = WaitingText;
        private string _statusText = string.Empty;
        private string _resultsText = string.Empty;

        private bool _isExpanded;
        private bool _isRunning;

        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;

        public void SetCounter(string text) => _counterText = text;

        public void SetResults(string text) => _resultsText = text;

        public void SetRunning(bool isRunning, string statusText)
        {
            _isRunning = isRunning;
            _statusText = statusText;
        }

        private void OnGUI()
        {
            if (_labelStyle == null)
                CreateStyles();

            // Размеры заданы для меньшей стороны экрана в 1080 точек: на телефоне панель не мельче, чем на ПК
            float scale = Mathf.Min(Screen.width, Screen.height) / ReferenceScreenSize;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));

            if (_isExpanded)
                DrawPanel();
            else
                DrawCollapsed();
        }

        private void DrawCollapsed()
        {
            Rect buttonRect = new(PanelLeft, PanelTop, CollapsedButtonWidth, ButtonHeight);

            if (GUI.Button(buttonRect, CollapsedButtonText, _buttonStyle))
                _isExpanded = true;
        }

        private void DrawPanel()
        {
            Rect panelRect = new(PanelLeft, PanelTop, PanelWidth, PanelHeight);

            Color previousColor = GUI.color;
            GUI.color = _backgroundColor;
            GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
            GUI.color = previousColor;

            GUILayout.BeginArea(new Rect(
                panelRect.x + PanelPadding,
                panelRect.y + PanelPadding,
                panelRect.width - PanelPadding * 2,
                panelRect.height - PanelPadding * 2));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(HideButtonText, _buttonStyle, GUILayout.Height(ButtonHeight)))
                _isExpanded = false;

            GUI.enabled = _isRunning == false;

            if (GUILayout.Button(RunButtonText, _buttonStyle, GUILayout.Height(ButtonHeight)))
                RunClicked?.Invoke();

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            GUILayout.Label(_counterText, _labelStyle);
            GUILayout.Label(_statusText, _labelStyle);
            GUILayout.Label(_resultsText, _labelStyle);

            GUILayout.EndArea();
        }

        private void CreateStyles()
        {
            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = FontSize, wordWrap = false };
            _buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = FontSize };
        }
    }
}
#endif
