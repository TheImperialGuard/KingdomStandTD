using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SummonFeature
{
    // Строй призыва — сетка вдоль центральной линии пути, то есть ломаной по вейпоинтам. Колонн столько же, сколько полос,
    // ряды идут через RowSpacing вперёд и назад от призывающего, его клетка в ряду 0 пропускается. Клетки занимаются от него
    // наружу: ближние ряды раньше дальних, в ряду ближние колонны раньше дальних, из равных — передний ряд и левая колонна.
    // Ряды за финишем пропускаются, их клетки переходят в следующие ряды. Расчёт идёт в плоскости XZ, высота клеток —
    // как у призывающего.
    public class SummonFormationLayout
    {
        // По числу PathLane: каждая колонна идёт своей полосой.
        private const int ColumnsCount = 3;

        // Ряд у самого финиша не ставится: призванный прорвался бы, ещё поднимаясь.
        private const float FinishMargin = 0.1f;

        private readonly List<Vector3> _pathPoints = new List<Vector3>();
        private readonly List<float> _pathDistances = new List<float>();
        private readonly Vector3 _directionBeforePath;

        private readonly float _summonerDistance;
        private readonly float _summonerHeight;

        private readonly float[] _columnOffsets = new float[ColumnsCount];
        private readonly PathLane[] _columnLanes = new PathLane[ColumnsCount];
        private readonly int _summonerColumn;

        private readonly float _rowSpacing;

        // nextPointIndex — индекс вейпоинта, к которому идёт призывающий, summonerTarget — эта точка с учётом его полосы.
        public SummonFormationLayout(
            Vector3 summonerPosition,
            Vector3 summonerTarget,
            IReadOnlyList<Vector3> pathPoints,
            int nextPointIndex,
            float summonerLaneOffset,
            float rowSpacing,
            float columnSpacing)
        {
            _summonerHeight = summonerPosition.y;
            _rowSpacing = rowSpacing;

            Vector3 summonerFlatPosition = Flatten(summonerPosition);

            FillPath(pathPoints);

            // Точки спавна в пути нет. Пока призывающий не дошёл до первого вейпоинта, линия продлевается назад по его ходу,
            // а после — по первому отрезку пути.
            _directionBeforePath = nextPointIndex == 0
                ? (Flatten(summonerTarget) - summonerFlatPosition).normalized
                : (_pathPoints[1] - _pathPoints[0]).normalized;

            _summonerDistance = GetDistanceAlongPath(summonerFlatPosition, nextPointIndex);

            FillColumns(summonerLaneOffset, columnSpacing);
            _summonerColumn = GetClosestColumn(summonerLaneOffset);
        }

        private float PathLength => _pathDistances[_pathDistances.Count - 1];

        public List<SummonFormationCell> GetCells(int count)
        {
            List<SummonFormationCell> cells = new List<SummonFormationCell>(count);

            for (int rowsDistance = 0; cells.Count < count; rowsDistance++)
            {
                foreach (SummonFormationCell cell in GetCellsAt(rowsDistance))
                {
                    if (cells.Count == count)
                        break;

                    cells.Add(cell);
                }
            }

            return cells;
        }

        private static Vector3 Flatten(Vector3 point) => new Vector3(point.x, 0, point.z);

        private static PathLane GetClosestLane(float offset)
        {
            PathLane closestLane = PathLane.Center;
            float closestDistance = float.MaxValue;

            foreach (PathLane lane in Enum.GetValues(typeof(PathLane)))
            {
                float distance = Mathf.Abs(PathLaneOffsets.ToLocalOffset(lane).x - offset);

                if (distance < closestDistance)
                {
                    closestLane = lane;
                    closestDistance = distance;
                }
            }

            return closestLane;
        }

        private void FillPath(IReadOnlyList<Vector3> pathPoints)
        {
            float distance = 0;

            foreach (Vector3 pathPoint in pathPoints)
            {
                Vector3 point = Flatten(pathPoint);

                if (_pathPoints.Count > 0)
                    distance += Vector3.Distance(_pathPoints[_pathPoints.Count - 1], point);

                _pathPoints.Add(point);
                _pathDistances.Add(distance);
            }
        }

        // Строй держится в пределах дороги: его центр сдвигается к полосе призывающего, насколько хватает ширины.
        private void FillColumns(float summonerLaneOffset, float columnSpacing)
        {
            float middleColumn = (ColumnsCount - 1) / 2f;
            float maxCenterOffset = PathLaneOffsets.LANE_OFFSET - middleColumn * columnSpacing;

            float center = maxCenterOffset > 0
                ? Mathf.Clamp(summonerLaneOffset, -maxCenterOffset, maxCenterOffset)
                : 0;

            for (int column = 0; column < ColumnsCount; column++)
            {
                _columnOffsets[column] = center + (column - middleColumn) * columnSpacing;
                _columnLanes[column] = GetClosestLane(_columnOffsets[column]);
            }
        }

        private int GetClosestColumn(float offset)
        {
            int closestColumn = 0;

            for (int column = 1; column < ColumnsCount; column++)
            {
                if (Mathf.Abs(_columnOffsets[column] - offset) < Mathf.Abs(_columnOffsets[closestColumn] - offset))
                    closestColumn = column;
            }

            return closestColumn;
        }

        // Расстояние вдоль пути от первого вейпоинта до проекции призывающего на центральную линию; до первого вейпоинта
        // оно отрицательное.
        private float GetDistanceAlongPath(Vector3 position, int nextPointIndex)
        {
            if (nextPointIndex == 0)
                return Mathf.Min(Vector3.Dot(position - _pathPoints[0], _directionBeforePath), 0);

            Vector3 segmentStart = _pathPoints[nextPointIndex - 1];
            Vector3 segment = _pathPoints[nextPointIndex] - segmentStart;

            float alongSegment = Vector3.Dot(position - segmentStart, segment.normalized);

            return _pathDistances[nextPointIndex - 1] + Mathf.Clamp(alongSegment, 0, segment.magnitude);
        }

        // Клетки рядов на расстоянии rowsDistance от призывающего — в порядке заполнения.
        private List<SummonFormationCell> GetCellsAt(int rowsDistance)
        {
            List<LinePoint> rows = new List<LinePoint>();

            float frontRowDistance = _summonerDistance + rowsDistance * _rowSpacing;

            if (rowsDistance == 0 || frontRowDistance <= PathLength - FinishMargin)
                rows.Add(GetPointAt(frontRowDistance));

            if (rowsDistance > 0)
                rows.Add(GetPointAt(_summonerDistance - rowsDistance * _rowSpacing));

            List<SummonFormationCell> cells = new List<SummonFormationCell>();

            // В ряду призывающего его собственная колонна занята им самим.
            int firstColumnDistance = rowsDistance == 0 ? 1 : 0;

            for (int columnDistance = firstColumnDistance; columnDistance < ColumnsCount; columnDistance++)
            {
                foreach (LinePoint row in rows)
                {
                    foreach (int column in GetColumnsAt(columnDistance))
                        cells.Add(CreateCell(row, column));
                }
            }

            return cells;
        }

        // Колонны на расстоянии columnDistance от колонны призывающего, левая раньше правой.
        private List<int> GetColumnsAt(int columnDistance)
        {
            List<int> columns = new List<int>();

            int leftColumn = _summonerColumn - columnDistance;
            int rightColumn = _summonerColumn + columnDistance;

            if (leftColumn >= 0)
                columns.Add(leftColumn);

            if (rightColumn != leftColumn && rightColumn < ColumnsCount)
                columns.Add(rightColumn);

            return columns;
        }

        // Точка центральной линии на расстоянии distance от первого вейпоинта, направление её отрезка и индекс вейпоинта
        // в его конце — с него призванный начнёт путь. До первого вейпоинта линия продлена прямой.
        private LinePoint GetPointAt(float distance)
        {
            if (distance <= 0 || _pathPoints.Count == 1)
                return new LinePoint(_pathPoints[0] + _directionBeforePath * distance, _directionBeforePath, 0);

            int segmentEnd = 1;

            while (segmentEnd < _pathPoints.Count - 1 && distance > _pathDistances[segmentEnd])
                segmentEnd++;

            Vector3 segmentStart = _pathPoints[segmentEnd - 1];
            Vector3 direction = (_pathPoints[segmentEnd] - segmentStart).normalized;

            return new LinePoint(
                segmentStart + direction * (distance - _pathDistances[segmentEnd - 1]),
                direction,
                segmentEnd);
        }

        private SummonFormationCell CreateCell(LinePoint row, int column)
        {
            Vector3 right = Vector3.Cross(Vector3.up, row.Direction);

            Vector3 position = row.Position + right * _columnOffsets[column];
            position.y = _summonerHeight;

            return new SummonFormationCell(position, row.FirstWaypointIndex, _columnLanes[column]);
        }

        private readonly struct LinePoint
        {
            public LinePoint(Vector3 position, Vector3 direction, int firstWaypointIndex)
            {
                Position = position;
                Direction = direction;
                FirstWaypointIndex = firstWaypointIndex;
            }

            public Vector3 Position { get; }

            public Vector3 Direction { get; }

            public int FirstWaypointIndex { get; }
        }
    }
}
