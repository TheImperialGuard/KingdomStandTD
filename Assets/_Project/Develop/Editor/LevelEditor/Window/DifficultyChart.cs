using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Кривая сложности кампании на Painter2D: столбик на каждый стейдж, столбики сгруппированы по уровням кампании
    // и подписаны их номерами. Показатель один, поэтому и цвет один; уровень, выбранный в таблице, остаётся в цвете,
    // остальные гаснут. Подсказка — у каждого столбика, щелчок открывает уровень на этом стейдже.
    public class DifficultyChart : ChartElement
    {
        private const string ROOT_CLASS = "difficulty-chart";
        private const string EMPTY_TEXT = "В кампании нет уровней со стейджами";
        private const string CLICK_HINT = "Щелчок — открыть стейдж в редакторе";
        private const string LINE_BREAK = "\n";

        private const float GUTTER_WIDTH = 48f;
        private const float RIGHT_PADDING = 12f;
        private const float TOP_PADDING = 10f;
        private const float LABELS_HEIGHT = 18f;
        private const float GROUP_GAP = 16f;

        // Столбик не толще 24 px и отделён от соседа зазором; остаток места — воздух.
        private const float MAX_BAR_WIDTH = 24f;
        private const float MIN_BAR_WIDTH = 2f;
        private const float BAR_GAP = 2f;
        private const float CORNER_RADIUS = 4f;
        private const float MIN_VISIBLE_HEIGHT = 0.5f;

        private const int MAX_TICKS = 5;
        private const float EMPTY_AXIS_MAX = 1f;
        private const float DECIMAL_BASE = 10f;

        // Подпись оси стоит по центру своей линии сетки: сдвиг вверх на половину высоты строки.
        private const float AXIS_LABEL_OFFSET = 7f;

        // Ширина символа подписи уровня — оценка: если «N. Имя» не помещается под группой, остаётся номер.
        private const float LABEL_CHAR_WIDTH = 6.5f;

        private const float HALF = 0.5f;

        private static readonly float[] _stepMultipliers = { 1f, 2f, 5f, 10f };

        public event Action<LevelConfig, int> StageClicked;

        private readonly List<Group> _groups = new();
        private readonly List<Bar> _bars = new();

        private DifficultyMeasure _measure;
        private LevelConfig _selectedLevel;
        private float _axisMax = EMPTY_AXIS_MAX;
        private float _axisStep = EMPTY_AXIS_MAX;
        private Bar _hoveredBar;
        private Vector2 _pointerPosition;

        public DifficultyChart()
        {
            AddToClassList(ROOT_CLASS);

            generateVisualContent += OnGenerateVisualContent;

            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            RegisterCallback<TooltipEvent>(OnTooltip);
        }

        private bool HasSelectedGroup => _groups.Any(group => group.Level.Config == _selectedLevel);

        // Уровни — в порядке кампании, у каждого хотя бы один стейдж.
        public void SetData(IReadOnlyList<LevelOverview> levels, DifficultyMeasure measure, LevelConfig selectedLevel)
        {
            _measure = measure;
            _selectedLevel = selectedLevel;
            _hoveredBar = null;
            _groups.Clear();
            _bars.Clear();

            foreach (LevelOverview level in levels)
                AddGroup(level);

            ChooseScale();
            MarkDirtyRepaint();
        }

        public void SetSelectedLevel(LevelConfig level)
        {
            _selectedLevel = level;
            MarkDirtyRepaint();
        }

        private static float GetValue(StageStats stats, DifficultyMeasure measure)
        {
            switch (measure)
            {
                case DifficultyMeasure.Health:
                    return stats.Health;

                case DifficultyMeasure.EffectiveHealthPhysic:
                    return stats.EffectiveHealthPhysic;

                case DifficultyMeasure.EffectiveHealthMagic:
                    return stats.EffectiveHealthMagic;

                case DifficultyMeasure.HealthPerSecond:
                    return stats.HealthPerSecond;

                default:
                    return 0f;
            }
        }

        // Шаг сетки — круглое число вида 1, 2 или 5 × 10ⁿ, не меньше нужного.
        private static float ChooseStep(float roughStep)
        {
            float magnitude = Mathf.Pow(DECIMAL_BASE, Mathf.Floor(Mathf.Log10(roughStep)));

            foreach (float multiplier in _stepMultipliers)
            {
                if (multiplier * magnitude >= roughStep)
                    return multiplier * magnitude;
            }

            return _stepMultipliers[_stepMultipliers.Length - 1] * magnitude;
        }

        // Столбик со скруглённым верхом и прямым основанием.
        private static void AddColumnPath(Painter2D painter, Rect rect)
        {
            float radius = Mathf.Min(CORNER_RADIUS, rect.width * HALF, rect.height);

            painter.MoveTo(new Vector2(rect.xMin, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMin + radius));
            painter.ArcTo(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin + radius, rect.yMin), radius);
            painter.LineTo(new Vector2(rect.xMax - radius, rect.yMin));
            painter.ArcTo(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMin + radius), radius);
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.ClosePath();
        }

        private static float GetPlotBottom(Rect area) => area.height - LABELS_HEIGHT;

        // Под группой — «N. Имя» уровня, если помещается, иначе только номер.
        private static string ChooseGroupLabel(Group group)
        {
            string number = group.Level.CampaignNumber.ToString();
            string fullLabel = $"{number}. {group.Level.Config.name}";
            float groupWidth = group.End - group.Start;

            return fullLabel.Length * LABEL_CHAR_WIDTH <= groupWidth ? fullLabel : number;
        }

        private void AddGroup(LevelOverview level)
        {
            Group group = new(level);
            IReadOnlyList<StageStats> stages = level.Stats.Stages;

            for (int i = 0; i < stages.Count; i++)
            {
                Bar bar = new(group, i, GetValue(stages[i], _measure));

                group.Bars.Add(bar);
                _bars.Add(bar);
            }

            _groups.Add(group);
        }

        // Верх шкалы — ближайшее сверху кратное круглого шага сетки. Бесконечный EHP (сопротивление 100 %)
        // в шкалу не входит: такой столбик рисуется во всю высоту.
        private void ChooseScale()
        {
            float maxValue = _bars
                .Select(bar => bar.Value)
                .Where(value => float.IsInfinity(value) == false)
                .DefaultIfEmpty(0f)
                .Max();

            if (maxValue <= 0f)
            {
                _axisStep = EMPTY_AXIS_MAX;
                _axisMax = EMPTY_AXIS_MAX;
                return;
            }

            _axisStep = ChooseStep(maxValue / MAX_TICKS);
            _axisMax = Mathf.Ceil(maxValue / _axisStep) * _axisStep;
        }

        private float GetBarHeight(float value, float plotHeight)
        {
            if (float.IsInfinity(value))
                return plotHeight;

            return Mathf.Clamp01(value / _axisMax) * plotHeight;
        }

        // Ширина места под столбик — поровну на все стейджи, между уровнями — промежуток. Зона наведения столбика —
        // всё его место по ширине и вся высота графика.
        private void LayoutBars(Rect area)
        {
            float plotLeft = GUTTER_WIDTH;
            float plotRight = area.width - RIGHT_PADDING;
            float plotBottom = GetPlotBottom(area);
            float plotHeight = plotBottom - TOP_PADDING;
            float barsWidth = Mathf.Max(0f, plotRight - plotLeft - _groups.Count * GROUP_GAP);
            float slotWidth = barsWidth / _bars.Count;
            float barWidth = Mathf.Clamp(slotWidth - BAR_GAP, MIN_BAR_WIDTH, MAX_BAR_WIDTH);
            float x = plotLeft + GROUP_GAP * HALF;

            foreach (Group group in _groups)
            {
                group.Start = x;

                foreach (Bar bar in group.Bars)
                {
                    float barLeft = x + (slotWidth - barWidth) * HALF;
                    float barHeight = GetBarHeight(bar.Value, plotHeight);

                    bar.Rect = Rect.MinMaxRect(barLeft, plotBottom - barHeight, barLeft + barWidth, plotBottom);
                    bar.HitArea = Rect.MinMaxRect(x, TOP_PADDING, x + slotWidth, plotBottom);
                    x += slotWidth;
                }

                group.End = x;
                x += GROUP_GAP;
            }
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Rect area = contentRect;

            if (area.width < MIN_DRAWABLE_SIZE || area.height < MIN_DRAWABLE_SIZE)
                return;

            Painter2D painter = context.painter2D;
            FillRect(painter, area, TimelinePalette.Surface);

            if (_bars.Count == 0)
            {
                DrawText(context, EMPTY_TEXT, new Vector2(TEXT_PADDING, TEXT_PADDING), LABEL_FONT_SIZE, TimelinePalette.MutedText);
                return;
            }

            LayoutBars(area);
            DrawGrid(context, painter, area);
            DrawBars(painter);
            DrawGroupLabels(context, area);
        }

        // Горизонтальная сетка по круглым значениям с подписями слева; линия нуля — ось.
        private void DrawGrid(MeshGenerationContext context, Painter2D painter, Rect area)
        {
            float plotBottom = GetPlotBottom(area);
            float plotHeight = plotBottom - TOP_PADDING;
            float plotRight = area.width - RIGHT_PADDING;
            int ticksCount = Mathf.RoundToInt(_axisMax / _axisStep);

            for (int i = 0; i <= ticksCount; i++)
            {
                float value = i * _axisStep;
                float y = Crisp(plotBottom - value / _axisMax * plotHeight);
                Color lineColor = i == 0 ? TimelinePalette.Axis : TimelinePalette.Gridline;
                string label = LevelEditorTexts.Number(value);

                DrawLine(painter, new Vector2(GUTTER_WIDTH, y), new Vector2(plotRight, y), lineColor, HAIRLINE_WIDTH);
                DrawText(context, label, new Vector2(TEXT_PADDING, y - AXIS_LABEL_OFFSET), AXIS_FONT_SIZE, TimelinePalette.MutedText);
            }
        }

        private void DrawBars(Painter2D painter)
        {
            bool hasSelectedGroup = HasSelectedGroup;

            foreach (Bar bar in _bars)
            {
                if (bar.Rect.height < MIN_VISIBLE_HEIGHT)
                    continue;

                bool isDimmed = hasSelectedGroup && bar.Group.Level.Config != _selectedLevel;

                painter.fillColor = isDimmed ? TimelinePalette.Dimmed : TimelinePalette.MainSeries;
                painter.BeginPath();
                AddColumnPath(painter, bar.Rect);
                painter.Fill();

                if (bar != _hoveredBar)
                    continue;

                painter.strokeColor = TimelinePalette.Hover;
                painter.lineWidth = HAIRLINE_WIDTH;
                painter.BeginPath();
                AddColumnPath(painter, bar.Rect);
                painter.Stroke();
            }
        }

        // Подпись выбранного уровня — ярче остальных.
        private void DrawGroupLabels(MeshGenerationContext context, Rect area)
        {
            float labelTop = GetPlotBottom(area) + TEXT_PADDING;

            foreach (Group group in _groups)
            {
                string label = ChooseGroupLabel(group);
                float labelWidth = label.Length * LABEL_CHAR_WIDTH;
                float labelLeft = (group.Start + group.End - labelWidth) * HALF;
                Color color = group.Level.Config == _selectedLevel ? TimelinePalette.Selection : TimelinePalette.Text;

                DrawText(context, label, new Vector2(labelLeft, labelTop), LABEL_FONT_SIZE, color);
            }
        }

        // Значение выбранного показателя — первой строкой, следом стейдж и остальные показатели.
        private string DescribeBar(Bar bar)
        {
            LevelOverview level = bar.Group.Level;
            StageStats stats = level.Stats.Stages[bar.StageIndex];
            string measureName = LevelEditorTexts.GetMeasureName(_measure);
            string value = LevelEditorTexts.Number(bar.Value);
            string address = LevelAddress.Format(bar.StageIndex);
            string health = LevelEditorTexts.Number(stats.Health);
            string physic = LevelEditorTexts.Number(stats.EffectiveHealthPhysic);
            string magic = LevelEditorTexts.Number(stats.EffectiveHealthMagic);
            string perSecond = LevelEditorTexts.Number(stats.HealthPerSecond);

            List<string> lines = new()
            {
                $"{measureName} {value}",
                $"{level.Config.name} · {address}: врагов {stats.EnemiesCount}, волн {stats.WavesCount}",
                $"HP {health} · EHP физ {physic} / маг {magic} · HP в секунду {perSecond}",
                $"Золото к началу стейджа {stats.GoldAtStartWithoutSkips}",
                CLICK_HINT,
            };

            return string.Join(LINE_BREAK, lines);
        }

        private Bar FindBarAt(Vector2 position) => _bars.FirstOrDefault(bar => bar.HitArea.Contains(position));

        private void OnPointerDown(PointerDownEvent pointerEvent)
        {
            if (pointerEvent.button != (int)MouseButton.LeftMouse)
                return;

            Bar bar = FindBarAt(pointerEvent.localPosition);

            if (bar != null)
                StageClicked?.Invoke(bar.Group.Level.Config, bar.StageIndex);
        }

        private void OnPointerMove(PointerMoveEvent pointerEvent)
        {
            _pointerPosition = pointerEvent.localPosition;

            Bar bar = FindBarAt(_pointerPosition);

            if (bar == _hoveredBar)
                return;

            _hoveredBar = bar;
            MarkDirtyRepaint();
        }

        private void OnPointerLeave(PointerLeaveEvent _)
        {
            if (_hoveredBar == null)
                return;

            _hoveredBar = null;
            MarkDirtyRepaint();
        }

        private void OnTooltip(TooltipEvent tooltipEvent)
        {
            Bar bar = FindBarAt(_pointerPosition);

            if (bar == null)
                return;

            tooltipEvent.tooltip = DescribeBar(bar);
            tooltipEvent.rect = this.LocalToWorld(bar.HitArea);
            tooltipEvent.StopImmediatePropagation();
        }

        // Уровень на графике: его столбики и границы группы по горизонтали.
        private class Group
        {
            public Group(LevelOverview level) => Level = level;

            public LevelOverview Level { get; }
            public List<Bar> Bars { get; } = new();
            public float Start { get; set; }
            public float End { get; set; }
        }

        // Столбик стейджа: значение показателя, прямоугольник столбика и зона наведения.
        private class Bar
        {
            public Bar(Group group, int stageIndex, float value)
            {
                Group = group;
                StageIndex = stageIndex;
                Value = value;
            }

            public Group Group { get; }
            public int StageIndex { get; }
            public float Value { get; }
            public Rect Rect { get; set; }
            public Rect HitArea { get; set; }
        }
    }
}
