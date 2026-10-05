using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Таймлайн волн на Painter2D. Слева подписи строк «дорога × полоса», сверху ось времени и полоса отметок:
    // номера стейджей в режиме уровня, «скип» и «конец» в режиме стейджа. Волна — полоска от первого до последнего
    // врага с засечками выходов в цвете врага; спавн после конца стейджа заштрихован. Клик по волне выбирает её,
    // в режиме уровня клик по пустому месту выбирает стейдж. Подсказки — у волн, подписей строк и отметок стейджей.
    public class LevelTimeline : VisualElement
    {
        private const string ROOT_CLASS = "level-timeline";
        private const string EMPTY_TEXT = "В уровне нет волн";
        private const string SKIP_LABEL = "скип";
        private const string END_LABEL = "конец";

        private const float GUTTER_WIDTH = 64f;
        private const float RIGHT_PADDING = 12f;
        private const float AXIS_HEIGHT = 16f;
        private const float MARKS_HEIGHT = 14f;
        private const float ROW_PADDING = 3f;
        private const float SUB_ROW_PITCH = 16f;
        private const float BAR_HEIGHT = 12f;
        private const float BOTTOM_PADDING = 4f;
        private const float EMPTY_HEIGHT = 40f;
        private const float MIN_DRAWABLE_SIZE = 1f;

        // Полоска стоит посередине подстроки: сверху и снизу по столько же пикселей.
        private const float BAR_OFFSET = 2f;

        private const float TEXT_PADDING = 3f;
        private const float AXIS_FONT_SIZE = 10f;
        private const float LABEL_FONT_SIZE = 11f;

        // Подписи оси идут не чаще, чем раз в столько пикселей; подпись не рисуется, если ей не хватает места справа.
        private const float MIN_AXIS_LABEL_SPACING = 48f;
        private const float AXIS_LABEL_WIDTH = 36f;
        private const float MARK_LABEL_WIDTH = 32f;

        private const float HAIRLINE_WIDTH = 1f;
        private const float BOUNDARY_WIDTH = 1.5f;
        private const float TICK_WIDTH = 2f;
        private const float MIN_BAR_WIDTH = 4f;
        private const float SELECTION_WIDTH = 2f;
        private const float HATCH_SPACING = 4f;
        private const float DASH_LENGTH = 3f;
        private const float DASH_GAP = 3f;
        private const float HIT_SLOP = 3f;

        // Тонкая линия ложится на середину пикселя, иначе она размывается на два пикселя.
        private const float HALF_PIXEL = 0.5f;

        // Рамка выбора ложится снаружи полоски: путь рамки отступает на половину её толщины.
        private const float SELECTION_OFFSET = 1f;

        private static readonly float[] _axisSteps = { 1f, 2f, 5f, 10f, 15f, 30f, 60f, 120f, 300f, 600f };

        public event Action<int, int> WaveClicked;
        public event Action<int> StageClicked;

        private readonly List<float> _rowTops = new();
        private readonly List<float> _rowHeights = new();

        private TimelineModel _model = new();
        private int _selectedStageIndex = LevelAddress.NO_INDEX;
        private int _selectedWaveIndex = LevelAddress.NO_INDEX;
        private TimelineWave _hoveredWave;
        private Vector2 _pointerPosition;

        public LevelTimeline()
        {
            AddToClassList(ROOT_CLASS);

            generateVisualContent += OnGenerateVisualContent;

            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            RegisterCallback<TooltipEvent>(OnTooltip);
        }

        private float RowsTop => AXIS_HEIGHT + MARKS_HEIGHT;

        public void SetModel(TimelineModel model, int selectedStageIndex, int selectedWaveIndex)
        {
            _model = model;
            _selectedStageIndex = selectedStageIndex;
            _selectedWaveIndex = selectedWaveIndex;
            _hoveredWave = null;

            LayoutRows();
            MarkDirtyRepaint();
        }

        private static float Crisp(float value) => Mathf.Floor(value) + HALF_PIXEL;

        private static float ChooseAxisStep(float pixelsPerSecond)
        {
            foreach (float step in _axisSteps)
            {
                if (step * pixelsPerSecond >= MIN_AXIS_LABEL_SPACING)
                    return step;
            }

            return _axisSteps[_axisSteps.Length - 1];
        }

        private static void AddRectPath(Painter2D painter, Rect rect)
        {
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
        }

        private static void FillRect(Painter2D painter, Rect rect, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            AddRectPath(painter, rect);
            painter.Fill();
        }

        private static void StrokeRect(Painter2D painter, Rect rect, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            AddRectPath(painter, rect);
            painter.Stroke();
        }

        private static void DrawLine(Painter2D painter, Vector2 from, Vector2 to, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.Stroke();
        }

        private static void DrawDashedVerticalLine(Painter2D painter, float x, float top, float bottom, Color color)
        {
            painter.strokeColor = color;
            painter.lineWidth = HAIRLINE_WIDTH;
            painter.BeginPath();

            for (float y = top; y < bottom; y += DASH_LENGTH + DASH_GAP)
            {
                painter.MoveTo(new Vector2(x, y));
                painter.LineTo(new Vector2(x, Mathf.Min(y + DASH_LENGTH, bottom)));
            }

            painter.Stroke();
        }

        // Штриховка под 45°. У Painter2D нет обрезки, поэтому каждый штрих обрезается по прямоугольнику вручную:
        // штрих — точки (начало + t, низ − t), t от нуля до высоты, а начало идёт с шагом от левого края минус высота.
        private static void DrawHatch(Painter2D painter, Rect rect, Color color)
        {
            if (rect.width <= 0f)
                return;

            painter.strokeColor = color;
            painter.lineWidth = HAIRLINE_WIDTH;
            painter.BeginPath();

            for (float lineStart = rect.xMin - rect.height; lineStart < rect.xMax; lineStart += HATCH_SPACING)
            {
                float fromOffset = Mathf.Max(0f, rect.xMin - lineStart);
                float toOffset = Mathf.Min(rect.height, rect.xMax - lineStart);

                if (fromOffset >= toOffset)
                    continue;

                painter.MoveTo(new Vector2(lineStart + fromOffset, rect.yMax - fromOffset));
                painter.LineTo(new Vector2(lineStart + toOffset, rect.yMax - toOffset));
            }

            painter.Stroke();
        }

        // Высота строки зависит от числа подстрок, высота таймлайна — от суммы строк; прокрутку даёт панель.
        private void LayoutRows()
        {
            _rowTops.Clear();
            _rowHeights.Clear();

            float top = RowsTop;

            foreach (TimelineRow row in _model.Rows)
            {
                float height = ROW_PADDING + row.SubRowsCount * SUB_ROW_PITCH + ROW_PADDING;

                _rowTops.Add(top);
                _rowHeights.Add(height);
                top += height;
            }

            style.height = _model.Rows.Count == 0 ? EMPTY_HEIGHT : top + BOTTOM_PADDING;
        }

        private float GetPixelsPerSecond()
        {
            float plotWidth = contentRect.width - GUTTER_WIDTH - RIGHT_PADDING;

            if (_model.Duration <= 0f || plotWidth <= 0f)
                return 0f;

            return plotWidth / _model.Duration;
        }

        private float ToX(float time, float pixelsPerSecond) => GUTTER_WIDTH + time * pixelsPerSecond;

        private Rect GetWaveRect(TimelineWave wave, float pixelsPerSecond)
        {
            float startX = ToX(wave.Start, pixelsPerSecond);
            float endX = Mathf.Max(ToX(wave.End, pixelsPerSecond) + TICK_WIDTH, startX + MIN_BAR_WIDTH);
            float top = _rowTops[wave.RowIndex] + ROW_PADDING + wave.SubRow * SUB_ROW_PITCH + BAR_OFFSET;

            return Rect.MinMaxRect(startX, top, endX, top + BAR_HEIGHT);
        }

        private Rect GetMarksRect(TimelineStage stage, float pixelsPerSecond)
            => Rect.MinMaxRect(ToX(stage.Start, pixelsPerSecond), AXIS_HEIGHT, ToX(stage.End, pixelsPerSecond), RowsTop);

        private bool IsSelected(TimelineWave wave)
            => wave.StageIndex == _selectedStageIndex && wave.WaveIndex == _selectedWaveIndex;

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Rect area = contentRect;

            if (area.width < MIN_DRAWABLE_SIZE || area.height < MIN_DRAWABLE_SIZE)
                return;

            Painter2D painter = context.painter2D;
            FillRect(painter, area, TimelinePalette.Surface);

            float pixelsPerSecond = GetPixelsPerSecond();

            if (_model.Rows.Count == 0 || pixelsPerSecond <= 0f)
            {
                DrawText(context, EMPTY_TEXT, new Vector2(TEXT_PADDING, TEXT_PADDING), LABEL_FONT_SIZE, TimelinePalette.MutedText);
                return;
            }

            DrawStageShades(painter, area, pixelsPerSecond);
            DrawGrid(painter, area, pixelsPerSecond);
            DrawStageMarks(painter, area, pixelsPerSecond);
            DrawWaves(painter, pixelsPerSecond);

            DrawAxisLabels(context, area, pixelsPerSecond);
            DrawMarkLabels(context, pixelsPerSecond);
            DrawRowLabels(context);
        }

        // Режим уровня: стейджи через один чуть темнее, выбранный — заметнее. Режим стейджа: время после конца стейджа.
        private void DrawStageShades(Painter2D painter, Rect area, float pixelsPerSecond)
        {
            if (_model.Mode == TimelineMode.Stage)
            {
                foreach (TimelineStage stage in _model.Stages)
                {
                    float endX = ToX(stage.End, pixelsPerSecond);

                    if (endX < area.xMax)
                        FillRect(painter, Rect.MinMaxRect(endX, AXIS_HEIGHT, area.xMax, area.yMax), TimelinePalette.Wash);
                }

                return;
            }

            bool isShaded = false;

            foreach (TimelineStage stage in _model.Stages)
            {
                Rect stageRect = Rect.MinMaxRect(ToX(stage.Start, pixelsPerSecond), AXIS_HEIGHT, ToX(stage.End, pixelsPerSecond), area.yMax);

                if (stage.StageIndex == _selectedStageIndex)
                    FillRect(painter, stageRect, TimelinePalette.StrongWash);
                else if (isShaded)
                    FillRect(painter, stageRect, TimelinePalette.Wash);

                isShaded = isShaded == false;
            }
        }

        // Вертикальная сетка по шагу оси, разделители строк, граница подписей и линия оси.
        private void DrawGrid(Painter2D painter, Rect area, float pixelsPerSecond)
        {
            float step = ChooseAxisStep(pixelsPerSecond);

            for (int i = 0; i * step <= _model.Duration; i++)
            {
                float x = Crisp(ToX(i * step, pixelsPerSecond));
                DrawLine(painter, new Vector2(x, AXIS_HEIGHT), new Vector2(x, area.yMax), TimelinePalette.Gridline, HAIRLINE_WIDTH);
            }

            for (int i = 0; i < _model.Rows.Count; i++)
            {
                float y = Crisp(_rowTops[i] + _rowHeights[i]);
                DrawLine(painter, new Vector2(0f, y), new Vector2(area.xMax, y), TimelinePalette.Gridline, HAIRLINE_WIDTH);
            }

            float gutterX = Crisp(GUTTER_WIDTH);
            float axisY = Crisp(RowsTop);

            DrawLine(painter, new Vector2(gutterX, AXIS_HEIGHT), new Vector2(gutterX, area.yMax), TimelinePalette.Axis, HAIRLINE_WIDTH);
            DrawLine(painter, new Vector2(0f, axisY), new Vector2(area.xMax, axisY), TimelinePalette.Axis, HAIRLINE_WIDTH);
        }

        // Конец стейджа — сплошная линия, скип — пунктир. В режиме уровня конец стейджа — граница со следующим.
        private void DrawStageMarks(Painter2D painter, Rect area, float pixelsPerSecond)
        {
            foreach (TimelineStage stage in _model.Stages)
            {
                float endX = Crisp(ToX(stage.End, pixelsPerSecond));
                DrawLine(painter, new Vector2(endX, AXIS_HEIGHT), new Vector2(endX, area.yMax), TimelinePalette.StageBoundary, BOUNDARY_WIDTH);

                if (stage.Skip.HasValue)
                {
                    float skipX = Crisp(ToX(stage.Skip.Value, pixelsPerSecond));
                    DrawDashedVerticalLine(painter, skipX, AXIS_HEIGHT, area.yMax, TimelinePalette.StageBoundary);
                }
            }
        }

        private void DrawWaves(Painter2D painter, float pixelsPerSecond)
        {
            foreach (TimelineWave wave in _model.Waves)
            {
                Rect bar = GetWaveRect(wave, pixelsPerSecond);

                FillRect(painter, bar, TimelinePalette.GetTrackColor(wave.TrackSlot));

                if (wave.OverflowStart.HasValue)
                {
                    float overflowX = Mathf.Max(bar.xMin, ToX(wave.OverflowStart.Value, pixelsPerSecond));
                    DrawHatch(painter, Rect.MinMaxRect(overflowX, bar.yMin, bar.xMax, bar.yMax), TimelinePalette.Warning);
                }

                DrawSpawnTicks(painter, wave, bar, pixelsPerSecond);

                if (IsSelected(wave))
                {
                    Rect ring = Rect.MinMaxRect(bar.xMin - SELECTION_OFFSET, bar.yMin - SELECTION_OFFSET, bar.xMax + SELECTION_OFFSET, bar.yMax + SELECTION_OFFSET);
                    StrokeRect(painter, ring, TimelinePalette.Selection, SELECTION_WIDTH);
                }
                else if (wave == _hoveredWave)
                {
                    StrokeRect(painter, bar, TimelinePalette.Hover, HAIRLINE_WIDTH);
                }
            }
        }

        // Засечки одного врага собираются в один путь — так на каждый цвет одна заливка.
        private void DrawSpawnTicks(Painter2D painter, TimelineWave wave, Rect bar, float pixelsPerSecond)
        {
            foreach (IGrouping<int, TimelineSpawn> slotSpawns in wave.Spawns.GroupBy(spawn => spawn.EnemySlot))
            {
                painter.fillColor = TimelinePalette.GetEnemyColor(slotSpawns.Key);
                painter.BeginPath();

                foreach (TimelineSpawn spawn in slotSpawns)
                {
                    float x = ToX(spawn.Time, pixelsPerSecond);
                    AddRectPath(painter, Rect.MinMaxRect(x, bar.yMin, x + TICK_WIDTH, bar.yMax));
                }

                painter.Fill();
            }
        }

        private void DrawAxisLabels(MeshGenerationContext context, Rect area, float pixelsPerSecond)
        {
            float step = ChooseAxisStep(pixelsPerSecond);

            for (int i = 0; i * step <= _model.Duration; i++)
            {
                float time = i * step;
                float x = ToX(time, pixelsPerSecond);

                if (x + AXIS_LABEL_WIDTH > area.xMax)
                    break;

                string label = LevelEditorTexts.Seconds(time);
                DrawText(context, label, new Vector2(x + TEXT_PADDING, 0f), AXIS_FONT_SIZE, TimelinePalette.MutedText);
            }
        }

        // Режим уровня — номера стейджей, режим стейджа — «скип» и «конец», если подписи хватает места.
        private void DrawMarkLabels(MeshGenerationContext context, float pixelsPerSecond)
        {
            foreach (TimelineStage stage in _model.Stages)
            {
                float endX = ToX(stage.End, pixelsPerSecond);

                if (_model.Mode == TimelineMode.Level)
                {
                    int stageNumber = stage.StageIndex + 1;
                    Color color = stage.StageIndex == _selectedStageIndex ? TimelinePalette.Selection : TimelinePalette.Text;
                    float startX = ToX(stage.Start, pixelsPerSecond);

                    DrawText(context, stageNumber.ToString(), new Vector2(startX + TEXT_PADDING, AXIS_HEIGHT), AXIS_FONT_SIZE, color);
                    continue;
                }

                if (stage.Skip.HasValue)
                {
                    float skipX = ToX(stage.Skip.Value, pixelsPerSecond);

                    if (endX - skipX >= MARK_LABEL_WIDTH)
                        DrawText(context, SKIP_LABEL, new Vector2(skipX + TEXT_PADDING, AXIS_HEIGHT), AXIS_FONT_SIZE, TimelinePalette.Text);
                }

                DrawText(context, END_LABEL, new Vector2(endX + TEXT_PADDING, AXIS_HEIGHT), AXIS_FONT_SIZE, TimelinePalette.Text);
            }
        }

        private void DrawRowLabels(MeshGenerationContext context)
        {
            for (int i = 0; i < _model.Rows.Count; i++)
            {
                Vector2 position = new(TEXT_PADDING, _rowTops[i] + ROW_PADDING);
                DrawText(context, _model.Rows[i].Label, position, LABEL_FONT_SIZE, TimelinePalette.Text);
            }
        }

        private void DrawText(MeshGenerationContext context, string text, Vector2 position, float fontSize, Color color)
            => context.DrawText(text, position, fontSize, color, resolvedStyle.unityFontDefinition.fontAsset);

        private TimelineWave FindWaveAt(Vector2 position)
        {
            float pixelsPerSecond = GetPixelsPerSecond();

            foreach (TimelineWave wave in _model.Waves)
            {
                Rect bar = GetWaveRect(wave, pixelsPerSecond);
                Rect hitArea = Rect.MinMaxRect(bar.xMin - HIT_SLOP, bar.yMin - BAR_OFFSET, bar.xMax + HIT_SLOP, bar.yMax + BAR_OFFSET);

                if (hitArea.Contains(position))
                    return wave;
            }

            return null;
        }

        // Стейдж под курсором — по времени, в области строк или отметок; правее последнего стейджа — ничего.
        private TimelineStage FindStageAt(Vector2 position)
        {
            float pixelsPerSecond = GetPixelsPerSecond();

            if (pixelsPerSecond <= 0f || position.x < GUTTER_WIDTH || position.y < AXIS_HEIGHT)
                return null;

            float time = (position.x - GUTTER_WIDTH) / pixelsPerSecond;

            return _model.Stages.FirstOrDefault(stage => time >= stage.Start && time < stage.End);
        }

        private int FindRowIndexAt(Vector2 position)
        {
            if (position.x >= GUTTER_WIDTH)
                return LevelAddress.NO_INDEX;

            for (int i = 0; i < _model.Rows.Count; i++)
            {
                if (position.y >= _rowTops[i] && position.y < _rowTops[i] + _rowHeights[i])
                    return i;
            }

            return LevelAddress.NO_INDEX;
        }

        // Подсказка живёт, пока курсор в её прямоугольнике, поэтому у стейджа это только полоса отметок:
        // иначе подсказка стейджа закрывала бы подсказки его волн.
        private bool TryDescribeAt(Vector2 position, out string text, out Rect area)
        {
            float pixelsPerSecond = GetPixelsPerSecond();
            TimelineWave wave = FindWaveAt(position);

            if (wave != null)
            {
                text = wave.Tooltip;
                area = GetWaveRect(wave, pixelsPerSecond);
                return true;
            }

            int rowIndex = FindRowIndexAt(position);

            if (rowIndex != LevelAddress.NO_INDEX)
            {
                text = _model.Rows[rowIndex].Tooltip;
                area = new Rect(0f, _rowTops[rowIndex], GUTTER_WIDTH, _rowHeights[rowIndex]);
                return true;
            }

            TimelineStage stage = FindStageAt(position);

            if (stage != null && position.y < RowsTop)
            {
                text = stage.Tooltip;
                area = GetMarksRect(stage, pixelsPerSecond);
                return true;
            }

            text = null;
            area = default;
            return false;
        }

        private void OnPointerDown(PointerDownEvent pointerEvent)
        {
            if (pointerEvent.button != (int)MouseButton.LeftMouse)
                return;

            Vector2 position = pointerEvent.localPosition;
            TimelineWave wave = FindWaveAt(position);

            if (wave != null)
            {
                WaveClicked?.Invoke(wave.StageIndex, wave.WaveIndex);
                return;
            }

            TimelineStage stage = FindStageAt(position);

            if (stage != null && _model.Mode == TimelineMode.Level)
                StageClicked?.Invoke(stage.StageIndex);
        }

        private void OnPointerMove(PointerMoveEvent pointerEvent)
        {
            _pointerPosition = pointerEvent.localPosition;

            TimelineWave wave = FindWaveAt(_pointerPosition);

            if (wave == _hoveredWave)
                return;

            _hoveredWave = wave;
            MarkDirtyRepaint();
        }

        private void OnPointerLeave(PointerLeaveEvent _)
        {
            if (_hoveredWave == null)
                return;

            _hoveredWave = null;
            MarkDirtyRepaint();
        }

        private void OnTooltip(TooltipEvent tooltipEvent)
        {
            if (TryDescribeAt(_pointerPosition, out string text, out Rect area) == false)
                return;

            tooltipEvent.tooltip = text;
            tooltipEvent.rect = this.LocalToWorld(area);
            tooltipEvent.StopImmediatePropagation();
        }
    }
}
