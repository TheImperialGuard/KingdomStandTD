using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.CameraManagment
{
    // Наклонная ортографическая камера над прямоугольным полем: двигается и меняет масштаб, не показывая ничего за его
    // границами. Дальний масштаб — вид, вписанный в границы, ближний — приближение к нему в заданное число раз.
    [RequireComponent(typeof(Camera))]
    public sealed class BoundedOrthoCamera : MonoBehaviour
    {
        // Вписываем видимую область чуть теснее границ, чтобы не балансировать
        // на точном равенстве и всегда попадать в обычную ветку ограничения.
        private const float FitSafetyFactor = 0.999f;
        private const float AspectChangeTolerance = 0.0001f;
        private const float NoZoom = 1f;

        private static readonly Vector2[] _viewportCorners =
        {
            new(0f, 0f),
            new(0f, 1f),
            new(1f, 1f),
            new(1f, 0f)
        };

        [Tooltip("Высота верхней грани тайлов поля: по ней край кадра держится на краю поля")]
        [SerializeField] private float _groundY;

        private readonly ReactiveEvent _moved = new();

        private Camera _camera;
        private Vector2 _worldMin;
        private Vector2 _worldMax;
        private float _fitSize;
        private float _lastAspect;
        private bool _isFitted;

        /// <summary>
        /// Камера сдвинулась или изменила масштаб: всё, что привязано на экране к точкам мира, пора переставить.
        /// </summary>
        public IReadOnlyEvent Moved => _moved;

        /// <summary>
        /// Задаёт границы поля на плоскости XZ, вписывает в них вид и ставит камеру по центру. Приближение сбрасывается.
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            _worldMin = Vector2.Min(min, max);
            _worldMax = Vector2.Max(min, max);
            _lastAspect = _camera.aspect;
            _isFitted = TryFitSize(NoZoom);

            CenterCamera();
        }

        /// <summary>
        /// Смена ориентации или разрешения меняет ширину видимой области: вид вписывается заново с тем же приближением,
        /// а положение перепроверяется. Вызывать каждый кадр.
        /// </summary>
        public void RefitOnAspectChange()
        {
            if (IsAspectChanged() == false)
                return;

            _lastAspect = _camera.aspect;

            if (_isFitted == false)
                return;

            TryFitSize(GetZoom());
            SetClampedPosition(transform.position);
        }

        /// <summary>
        /// Сдвигает камеру на delta в мировых координатах, не меняя её высоту.
        /// </summary>
        public void Move(Vector3 delta)
        {
            delta.y = 0f;

            SetClampedPosition(transform.position + delta);
        }

        /// <summary>
        /// Меняет масштаб в factor раз (меньше 1 — приближение), оставляя на месте точку поля под screenPoint.
        /// Масштаб не выходит за пределы: от вида, вписанного в границы, до приближения к нему в maxZoom раз.
        /// </summary>
        public void Zoom(float factor, Vector2 screenPoint, float maxZoom)
        {
            if (_isFitted == false)
                return;

            float minSize = _fitSize / Mathf.Max(maxZoom, NoZoom);
            float size = Mathf.Clamp(_camera.orthographicSize * factor, minSize, _fitSize);

            if (Mathf.Approximately(size, _camera.orthographicSize))
                return;

            Vector3 position = transform.position;
            bool hasPivot = TryGetGroundPoint(_camera.ScreenPointToRay(screenPoint), out Vector3 pivotBefore);

            _camera.orthographicSize = size;

            if (hasPivot && TryGetGroundPoint(_camera.ScreenPointToRay(screenPoint), out Vector3 pivotAfter))
                position += pivotBefore - pivotAfter;

            SetClampedPosition(position);
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            if (_camera.orthographic == false)
                Debug.LogError($"{nameof(BoundedOrthoCamera)} requires an orthographic camera.", this);
        }

        private void CenterCamera()
        {
            if (TryGetVisibleRect(out Rect visible) == false)
                return;

            Vector2 offset = (_worldMin + _worldMax) / 2f - visible.center;

            SetClampedPosition(transform.position + new Vector3(offset.x, 0f, offset.y));
        }

        private void SetClampedPosition(Vector3 position)
        {
            transform.position = ClampPosition(position);
            _moved.Invoke();
        }

        // Сдвиг ортографической камеры по горизонтали сдвигает видимую область ровно на столько же,
        // поэтому область считается в текущем положении и переносится в целевое.
        private Vector3 ClampPosition(Vector3 targetPosition)
        {
            Vector3 currentPosition = transform.position;

            targetPosition.y = currentPosition.y;

            if (TryGetVisibleRect(out Rect visible) == false)
                return currentPosition;

            Vector3 offset = targetPosition - currentPosition;
            visible.position += new Vector2(offset.x, offset.z);

            targetPosition.x += GetClampOffset(visible.xMin, visible.xMax, _worldMin.x, _worldMax.x);
            targetPosition.z += GetClampOffset(visible.yMin, visible.yMax, _worldMin.y, _worldMax.y);

            return targetPosition;
        }

        // Видимые размеры линейны по orthographicSize при любом наклоне камеры,
        // поэтому вписанный размер получается масштабированием текущего.
        private bool TryFitSize(float zoom)
        {
            if (TryGetVisibleRect(out Rect visible) == false || visible.width <= 0f || visible.height <= 0f)
                return false;

            float boundsWidth = _worldMax.x - _worldMin.x;
            float boundsDepth = _worldMax.y - _worldMin.y;
            float scale = Mathf.Min(boundsWidth / visible.width, boundsDepth / visible.height);

            _fitSize = _camera.orthographicSize * scale * FitSafetyFactor;
            _camera.orthographicSize = _fitSize / zoom;

            return true;
        }

        // Прямоугольник, который камера видит на плоскости поля: x — мировая ось X, y — мировая ось Z.
        private bool TryGetVisibleRect(out Rect rect)
        {
            rect = default;

            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;

            foreach (Vector2 viewportCorner in _viewportCorners)
            {
                if (TryGetGroundPoint(_camera.ViewportPointToRay(viewportCorner), out Vector3 point) == false)
                    return false;

                Vector2 groundPoint = new(point.x, point.z);

                min = Vector2.Min(min, groundPoint);
                max = Vector2.Max(max, groundPoint);
            }

            rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            return true;
        }

        private bool TryGetGroundPoint(Ray ray, out Vector3 point)
        {
            Plane ground = new(Vector3.up, new Vector3(0f, _groundY, 0f));

            if (ground.Raycast(ray, out float distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }

            point = Vector3.zero;
            return false;
        }

        private float GetZoom() => _fitSize / _camera.orthographicSize;

        private bool IsAspectChanged() => Mathf.Abs(_camera.aspect - _lastAspect) > AspectChangeTolerance;

        // Если камера видит больше, чем есть поля, она встаёт по центру этой оси,
        // иначе край видимой области подтягивается к границе.
        private static float GetClampOffset(float visibleMin, float visibleMax, float boundsMin, float boundsMax)
        {
            if (visibleMax - visibleMin >= boundsMax - boundsMin)
                return (boundsMin + boundsMax) / 2f - (visibleMin + visibleMax) / 2f;

            if (visibleMin < boundsMin)
                return boundsMin - visibleMin;

            if (visibleMax > boundsMax)
                return boundsMax - visibleMax;

            return 0f;
        }
    }
}
