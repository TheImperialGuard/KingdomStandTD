using System.Collections.Generic;
using Assets._Project.Develop.Editor.LevelEditor.Window;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets._Project.Develop.Editor.LevelEditor.Roads
{
    // Рисует дороги в Scene View: центр дороги со стрелками по ходу врагов, полосы Л и П — ровно по маршрутам врагов,
    // спавн с подписью дороги и длинами полос, базу. Работает там, где есть дороги: в Prefab Mode префаба уровня
    // и в Play Mode. Подчиняется кнопке Gizmos в Scene View и галочкам панели «Дороги уровня».
    [InitializeOnLoad]
    public static class RoadsSceneDrawer
    {
        // Толщина линий — в пикселях экрана. Тёмная обводка шире линии на OUTLINE_EXTRA_WIDTH.
        private const float CENTER_LINE_WIDTH = 3.5f;
        private const float LANE_LINE_WIDTH = 2f;
        private const float OUTLINE_EXTRA_WIDTH = 2f;
        private const float MARKER_OUTLINE_WIDTH = 1.5f;

        // Значки и отступы подписей — в долях размера манипулятора Unity (HandleUtility.GetHandleSize): на экране
        // они одного размера при любом приближении.
        private const float ARROW_LENGTH = 0.1f;
        private const float ARROW_HALF_WIDTH = 0.065f;
        private const float SPAWN_RADIUS = 0.06f;
        private const float BASE_HALF_SIZE = 0.07f;
        private const float LETTERS_DISTANCE = 0.25f;
        private const float ROAD_LABEL_DISTANCE = 0.45f;
        private const float BASE_LABEL_DISTANCE = 0.2f;

        // Буквы полос рисуются, только когда соседние полосы на экране разошлись хотя бы на столько пикселей, — иначе
        // буквы налезают друг на друга.
        private const float MIN_LETTERS_SPACING = 16f;

        // Концы дорог ближе этого — одна база.
        private const float SAME_BASE_DISTANCE = 0.05f;

        private const int LABEL_FONT_SIZE = 11;
        private const int WAYPOINT_NUMBER_FONT_SIZE = 10;
        private const int TEXT_PADDING_X = 3;
        private const int TEXT_PADDING_Y = 1;
        private const float HALF = 0.5f;

        private const string BASE_TEXT = "База";
        private const string SEPARATOR = " · ";
        private const string LINE_BREAK = "\n";

        private static readonly PathLane[] _lanes = { PathLane.Left, PathLane.Center, PathLane.Right };
        private static readonly PathLane[] _sideLanes = { PathLane.Left, PathLane.Right };

        // В подписи дороги первой идёт длина по центру.
        private static readonly PathLane[] _lengthLanes = { PathLane.Center, PathLane.Left, PathLane.Right };

        private static readonly List<RoadShape> _shapes = new();
        private static readonly GUIContent _content = new();

        private static GUIStyle _textStyle;

        static RoadsSceneDrawer() => SceneView.duringSceneGui += OnSceneGui;

        private static void OnSceneGui(SceneView sceneView)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (sceneView.drawGizmos == false || RoadsSceneSettings.AreRoadsShown == false)
                return;

            BuildShapes();

            if (_shapes.Count == 0)
                return;

            CompareFunction previousZTest = Handles.zTest;

            // Дороги видны сквозь дома и деревья.
            Handles.zTest = CompareFunction.Always;

            // Цвет и матрица Handles вернутся к прежним после рисования.
            using (new Handles.DrawingScope(Handles.color))
            {
                if (RoadsSceneSettings.AreLanesShown)
                    DrawLanes();

                DrawCenterLines();
                DrawMarkers();
            }

            Handles.zTest = previousZTest;

            DrawTexts();
        }

        // Рисуются только целые пути: от двух вейпоинтов и без пустых.
        private static void BuildShapes()
        {
            _shapes.Clear();

            foreach (RoadPath road in StageRoads.Get())
            {
                if (RoadLanes.IsComplete(road.Waypoints))
                    _shapes.Add(new RoadShape(road, _lanes));
            }
        }

        private static void DrawLanes()
        {
            foreach (RoadShape shape in _shapes)
            {
                foreach (PathLane lane in _sideLanes)
                    DrawOutlinedLine(shape.GetPoints(lane), shape.Color, LANE_LINE_WIDTH);
            }
        }

        private static void DrawCenterLines()
        {
            foreach (RoadShape shape in _shapes)
            {
                Vector3[] center = shape.Center;

                DrawOutlinedLine(center, shape.Color, CENTER_LINE_WIDTH);

                for (int i = 1; i < center.Length; i++)
                    DrawArrow(center[i - 1], center[i], shape.Color);
            }
        }

        private static void DrawOutlinedLine(Vector3[] points, Color color, float width)
        {
            Handles.color = RoadPalette.Outline;
            Handles.DrawAAPolyLine(width + OUTLINE_EXTRA_WIDTH, points);

            Handles.color = color;
            Handles.DrawAAPolyLine(width, points);
        }

        // Стрелка посередине отрезка — по ходу врагов.
        private static void DrawArrow(Vector3 from, Vector3 to, Color color)
        {
            Vector3 direction = (to - from).normalized;

            if (direction == Vector3.zero)
                return;

            Vector3 middle = Vector3.Lerp(from, to, HALF);
            float size = HandleUtility.GetHandleSize(middle);
            Vector3 side = Vector3.Cross(Vector3.up, direction) * (ARROW_HALF_WIDTH * size);
            Vector3 halfLength = direction * (ARROW_LENGTH * HALF * size);

            Vector3 tip = middle + halfLength;
            Vector3 left = middle - halfLength - side;
            Vector3 right = middle - halfLength + side;

            Handles.color = color;
            Handles.DrawAAConvexPolygon(left, tip, right);

            Handles.color = RoadPalette.Outline;
            Handles.DrawAAPolyLine(MARKER_OUTLINE_WIDTH, left, tip, right, left);
        }

        private static void DrawMarkers()
        {
            for (int i = 0; i < _shapes.Count; i++)
            {
                DrawSpawn(_shapes[i]);

                if (IsFirstAtBase(i))
                    DrawBase(_shapes[i]);
            }
        }

        private static void DrawSpawn(RoadShape shape)
        {
            float radius = SPAWN_RADIUS * HandleUtility.GetHandleSize(shape.Start);

            Handles.color = shape.Color;
            Handles.DrawSolidDisc(shape.Start, Vector3.up, radius);

            Handles.color = RoadPalette.Outline;
            Handles.DrawWireDisc(shape.Start, Vector3.up, radius, MARKER_OUTLINE_WIDTH);
        }

        private static void DrawBase(RoadShape shape)
        {
            Vector3 end = shape.End;
            float halfSize = BASE_HALF_SIZE * HandleUtility.GetHandleSize(end);
            Vector3 forward = shape.EndDirection * halfSize;
            Vector3 side = Vector3.Cross(Vector3.up, shape.EndDirection) * halfSize;

            Vector3[] corners =
            {
                end - forward - side,
                end + forward - side,
                end + forward + side,
                end - forward + side,
            };

            Handles.DrawSolidRectangleWithOutline(corners, RoadPalette.Base, RoadPalette.Outline);
        }

        // База рисуется один раз, даже если в неё сходятся несколько дорог.
        private static bool IsFirstAtBase(int shapeIndex)
        {
            Vector3 end = _shapes[shapeIndex].End;

            for (int i = 0; i < shapeIndex; i++)
            {
                if (Vector3.Distance(_shapes[i].End, end) < SAME_BASE_DISTANCE)
                    return false;
            }

            return true;
        }

        private static void DrawTexts()
        {
            bool areLabelsShown = RoadsSceneSettings.AreLabelsShown;
            bool areWaypointNumbersShown = RoadsSceneSettings.AreWaypointNumbersShown;

            if (areLabelsShown == false && areWaypointNumbersShown == false)
                return;

            EnsureTextStyle();
            Handles.BeginGUI();

            for (int i = 0; i < _shapes.Count; i++)
            {
                RoadShape shape = _shapes[i];

                if (areLabelsShown)
                {
                    if (RoadsSceneSettings.AreLanesShown)
                        DrawLaneLetters(shape);

                    DrawRoadLabel(shape);

                    if (IsFirstAtBase(i))
                        DrawBaseLabel(shape);
                }

                if (areWaypointNumbersShown)
                    DrawWaypointNumbers(shape);
            }

            Handles.EndGUI();
        }

        // Буквы стоят перед спавном, каждая — на продолжении своей полосы.
        private static void DrawLaneLetters(RoadShape shape)
        {
            Vector3 offset = -shape.StartDirection * (LETTERS_DISTANCE * HandleUtility.GetHandleSize(shape.Start));

            bool isLeftVisible = TryGetGuiPoint(shape.GetPoints(PathLane.Left)[0] + offset, out Vector2 left);
            bool isRightVisible = TryGetGuiPoint(shape.GetPoints(PathLane.Right)[0] + offset, out Vector2 right);

            if (isLeftVisible == false || isRightVisible == false)
                return;

            if (Vector2.Distance(left, right) * HALF < MIN_LETTERS_SPACING)
                return;

            foreach (PathLane lane in _lanes)
            {
                if (TryGetGuiPoint(shape.GetPoints(lane)[0] + offset, out Vector2 point))
                    DrawLabel(point, LevelEditorTexts.GetLaneLetter(lane), TextAnchor.MiddleCenter, LABEL_FONT_SIZE, FontStyle.Bold);
            }
        }

        // Подпись дороги — перед спавном, по ту сторону от дороги.
        private static void DrawRoadLabel(RoadShape shape)
        {
            float distance = ROAD_LABEL_DISTANCE * HandleUtility.GetHandleSize(shape.Start);
            Vector3 labelPosition = shape.Start - shape.StartDirection * distance;

            if (TryGetGuiPoint(shape.Start, out Vector2 start) == false || TryGetGuiPoint(labelPosition, out Vector2 point) == false)
                return;

            DrawLabel(point, DescribeRoad(shape.Road), GetAnchorTowards(point - start), LABEL_FONT_SIZE, FontStyle.Normal);
        }

        private static void DrawBaseLabel(RoadShape shape)
        {
            float distance = BASE_LABEL_DISTANCE * HandleUtility.GetHandleSize(shape.End);
            Vector3 labelPosition = shape.End + shape.EndDirection * distance;

            if (TryGetGuiPoint(shape.End, out Vector2 end) == false || TryGetGuiPoint(labelPosition, out Vector2 point) == false)
                return;

            DrawLabel(point, BASE_TEXT, GetAnchorTowards(point - end), LABEL_FONT_SIZE, FontStyle.Normal);
        }

        // Номер вейпоинта в списке RoadPath — с нуля, как в инспекторе.
        private static void DrawWaypointNumbers(RoadShape shape)
        {
            Vector3[] center = shape.Center;

            for (int i = 0; i < center.Length; i++)
            {
                if (TryGetGuiPoint(center[i], out Vector2 point))
                    DrawLabel(point, i.ToString(), TextAnchor.MiddleCenter, WAYPOINT_NUMBER_FONT_SIZE, FontStyle.Normal);
            }
        }

        // «First · Path_1» и длины полос: «Ц 43 · Л 42.3 · П 43.7» — тем же расчётом, что в срезах.
        private static string DescribeRoad(RoadPath road)
        {
            List<string> lengths = new();

            foreach (PathLane lane in _lengthLanes)
            {
                string letter = LevelEditorTexts.GetLaneLetter(lane);
                string length = LevelEditorTexts.Number(RoadLanes.CalculateLength(road.Waypoints, lane));

                lengths.Add($"{letter} {length}");
            }

            string lengthsLine = string.Join(SEPARATOR, lengths);

            return $"{road.Number}{SEPARATOR}{road.name}{LINE_BREAK}{lengthsLine}";
        }

        private static void DrawLabel(Vector2 point, string text, TextAnchor anchor, int fontSize, FontStyle fontStyle)
        {
            _content.text = text;
            _textStyle.fontSize = fontSize;
            _textStyle.fontStyle = fontStyle;
            _textStyle.alignment = anchor;

            Rect rect = PlaceRect(point, _textStyle.CalcSize(_content), anchor);

            EditorGUI.DrawRect(rect, RoadPalette.LabelBackground);
            GUI.Label(rect, _content, _textStyle);
        }

        private static void EnsureTextStyle()
        {
            if (_textStyle != null)
                return;

            _textStyle = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(TEXT_PADDING_X, TEXT_PADDING_X, TEXT_PADDING_Y, TEXT_PADDING_Y),
                margin = new RectOffset(),
                richText = false,
                wordWrap = false,
                clipping = TextClipping.Overflow,
            };

            _textStyle.normal.textColor = RoadPalette.Text;
        }

        private static bool TryGetGuiPoint(Vector3 position, out Vector2 guiPoint)
        {
            Vector3 point = HandleUtility.WorldToGUIPointWithDepth(position);

            guiPoint = point;

            return point.z > 0f;
        }

        // Подпись уходит от точки в сторону направления на экране, чтобы не ложиться на дорогу. Ось Y GUI — вниз.
        private static TextAnchor GetAnchorTowards(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return direction.x > 0f ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;

            return direction.y > 0f ? TextAnchor.UpperCenter : TextAnchor.LowerCenter;
        }

        // Прямоугольник подписи, у которого точка привязки anchor совпадает с point.
        private static Rect PlaceRect(Vector2 point, Vector2 size, TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.MiddleLeft:
                    return new Rect(point.x, point.y - size.y * HALF, size.x, size.y);

                case TextAnchor.MiddleRight:
                    return new Rect(point.x - size.x, point.y - size.y * HALF, size.x, size.y);

                case TextAnchor.UpperCenter:
                    return new Rect(point.x - size.x * HALF, point.y, size.x, size.y);

                case TextAnchor.LowerCenter:
                    return new Rect(point.x - size.x * HALF, point.y - size.y, size.x, size.y);

                default:
                    return new Rect(point.x - size.x * HALF, point.y - size.y * HALF, size.x, size.y);
            }
        }

        // Точки дороги для одной перерисовки: центр и полосы — по тем же формулам, что маршрут врага.
        private class RoadShape
        {
            private readonly Dictionary<PathLane, Vector3[]> _lanePoints = new();

            public RoadShape(RoadPath road, IEnumerable<PathLane> lanes)
            {
                Road = road;
                Color = RoadPalette.GetRoadColor(road.Number);

                IReadOnlyList<Waypoint> waypoints = road.Waypoints;

                foreach (PathLane lane in lanes)
                {
                    Vector3[] points = new Vector3[waypoints.Count];

                    for (int i = 0; i < waypoints.Count; i++)
                        points[i] = RoadLanes.GetPoint(waypoints[i], lane);

                    _lanePoints[lane] = points;
                }

                Vector3[] center = Center;

                Start = center[0];
                End = center[center.Length - 1];
                StartDirection = (center[1] - Start).normalized;
                EndDirection = (End - center[center.Length - 2]).normalized;
            }

            public RoadPath Road { get; }
            public Color Color { get; }
            public Vector3 Start { get; }
            public Vector3 End { get; }
            public Vector3 StartDirection { get; }
            public Vector3 EndDirection { get; }
            public Vector3[] Center => _lanePoints[PathLane.Center];

            public Vector3[] GetPoints(PathLane lane) => _lanePoints[lane];
        }
    }
}
