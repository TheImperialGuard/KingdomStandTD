using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Основа графиков окна на Painter2D — таймлайна и кривой сложности: общие размеры текста и приёмы рисования.
    public abstract class ChartElement : VisualElement
    {
        protected const float MIN_DRAWABLE_SIZE = 1f;
        protected const float TEXT_PADDING = 3f;
        protected const float AXIS_FONT_SIZE = 10f;
        protected const float LABEL_FONT_SIZE = 11f;
        protected const float HAIRLINE_WIDTH = 1f;

        // Тонкая линия ложится на середину пикселя, иначе она размывается на два пикселя.
        private const float HALF_PIXEL = 0.5f;

        protected static float Crisp(float value) => Mathf.Floor(value) + HALF_PIXEL;

        protected static void AddRectPath(Painter2D painter, Rect rect)
        {
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
        }

        protected static void FillRect(Painter2D painter, Rect rect, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            AddRectPath(painter, rect);
            painter.Fill();
        }

        protected static void StrokeRect(Painter2D painter, Rect rect, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            AddRectPath(painter, rect);
            painter.Stroke();
        }

        protected static void DrawLine(Painter2D painter, Vector2 from, Vector2 to, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.Stroke();
        }

        protected void DrawText(MeshGenerationContext context, string text, Vector2 position, float fontSize, Color color)
            => context.DrawText(text, position, fontSize, color, resolvedStyle.unityFontDefinition.fontAsset);
    }
}
