using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class BoundedOrthoCamera : MonoBehaviour
{
    // Вписываем видимую область чуть теснее границ, чтобы не балансировать
    // на точном равенстве и всегда попадать в обычную ветку ограничения.
    private const float FIT_SAFETY_FACTOR = 0.999f;
    private const float ASPECT_CHANGE_TOLERANCE = 0.0001f;

    public ReactiveEvent CameraMoved = new();

    [Header("Границы игровой области на плоскости XZ")]
    [SerializeField] private Vector2 worldMin = new(-10f, -10f);
    [SerializeField] private Vector2 worldMax = new(10f, 10f);

    [Header("Плоскость игровой поверхности")]
    [SerializeField] private float groundY = 0f;

    [Header("Пределы масштаба камеры")]
    [SerializeField] private float _minOrthographicSize = 1f;
    [SerializeField] private float _maxOrthographicSize = 30f;

    [Header("Отладка")]
    [SerializeField] private bool drawGizmos = true;

    private Camera _camera;
    private float _fixedCameraY;
    private float _fixedCameraRotationX;
    private float _fixedCameraRotationZ;
    private float _lastAspect;

    private readonly Vector3[] _visibleCorners = new Vector3[4];

    private enum Corner
    {
        BottomLeft,
        TopLeft,
        TopRight,
        BottomRight
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (!_camera.orthographic)
        {
            Debug.LogError(
                $"{nameof(BoundedOrthoCamera)} requires an orthographic camera.",
                this);
        }

        SaveFixedCameraSettings();

        _lastAspect = _camera.aspect;
    }

    private void Start()
    {
        CenterCamera();
    }

    private void LateUpdate()
    {
        // Смена ориентации или разрешения меняет ширину видимой области,
        // поэтому масштаб пересчитывается, а положение перепроверяется.
        if (IsAspectChanged())
        {
            _lastAspect = _camera.aspect;
            FitSizeToBounds();
        }

        ClampCurrentPosition();
    }

    private void SaveFixedCameraSettings()
    {
        _fixedCameraY = transform.position.y;
        _fixedCameraRotationX = transform.eulerAngles.x;
        _fixedCameraRotationZ = transform.eulerAngles.z;
    }

    /// <summary>
    /// Перемещает камеру на delta в мировых координатах.
    /// Обычно delta следует умножать на скорость и Time.deltaTime
    /// во внешнем контроллере управления.
    /// </summary>
    public void MoveCamera(Vector3 delta)
    {
        Vector3 targetPosition = transform.position + delta;
        targetPosition.y = _fixedCameraY;

        transform.position = ClampPosition(targetPosition);
        CameraMoved?.Invoke();
    }

    /// <summary>
    /// Устанавливает позицию камеры, сохраняя допустимую высоту.
    /// </summary>
    public void SetCameraPosition(Vector3 position)
    {
        position.y = _fixedCameraY;
        transform.position = ClampPosition(position);
    }

    /// <summary>
    /// Центрирует фактическую видимую область камеры относительно центра границ.
    /// </summary>
    [ContextMenu("CenterCamera")]
    public void CenterCamera()
    {
        Vector2 boundsCenter = GetBoundsCenter();

        Vector3 currentPosition = transform.position;
        currentPosition.y = _fixedCameraY;

        // Сначала вычисляем положение центра видимой области
        // при текущей позиции камеры.
        Vector3 currentVisibleCenter = GetVisibleAreaCenter(currentPosition);

        // Перемещаем камеру на разницу между желаемым
        // и текущим центром видимой области.
        Vector3 delta = new Vector3(
            boundsCenter.x - currentVisibleCenter.x,
            0f,
            boundsCenter.y - currentVisibleCenter.z
        );

        Vector3 targetPosition = currentPosition + delta;
        targetPosition.y = _fixedCameraY;

        transform.position = ClampPosition(targetPosition);
    }

    /// <summary>
    /// Устанавливает новые границы и центрирует камеру.
    /// </summary>
    public void SetBounds(Vector2 newMin, Vector2 newMax)
    {
        worldMin = newMin;
        worldMax = newMax;

        NormalizeBounds();
        FitSizeToBounds();
        CenterCamera();
    }

    /// <summary>
    /// Подбирает размер камеры так, чтобы видимая область целиком помещалась в границы.
    /// </summary>
    [ContextMenu("FitSizeToBounds")]
    public void FitSizeToBounds()
    {
        if (TryCalculateFitSize(out float size) == false)
            return;

        if (Mathf.Approximately(_camera.orthographicSize, size))
            return;

        _camera.orthographicSize = size;

        CameraMoved?.Invoke();
    }

    /// <summary>
    /// Возвращает фактические четыре угла изображения,
    /// спроецированные на горизонтальную плоскость XZ.
    /// </summary>
    public bool TryGetVisibleCorners(
        Vector3 cameraPosition,
        Vector3[] corners)
    {
        if (corners == null || corners.Length < 4)
        {
            Debug.LogError("The corners array must contain at least four elements.");
            return false;
        }

        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;

        transform.position = cameraPosition;
        transform.rotation = Quaternion.Euler(
            _fixedCameraRotationX,
            transform.eulerAngles.y,
            _fixedCameraRotationZ
        );

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));

        Vector2[] viewportPoints =
        {
            new Vector2(0f, 0f), // BottomLeft
            new Vector2(0f, 1f), // TopLeft
            new Vector2(1f, 1f), // TopRight
            new Vector2(1f, 0f)  // BottomRight
        };

        bool success = true;

        for (int i = 0; i < viewportPoints.Length; i++)
        {
            Ray ray = _camera.ViewportPointToRay(viewportPoints[i]);

            if (groundPlane.Raycast(ray, out float distance))
            {
                corners[i] = ray.GetPoint(distance);
            }
            else
            {
                success = false;
            }
        }

        transform.position = savedPosition;
        transform.rotation = savedRotation;

        return success;
    }

    private Vector3 ClampPosition(Vector3 targetPosition)
    {
        targetPosition.y = _fixedCameraY;

        // Для ограничения используем фактические четыре угла
        // будущей видимой области.
        if (TryGetVisibleRect(targetPosition, out Vector2 visibleMin, out Vector2 visibleMax) == false)
        {
            return transform.position;
        }

        float boundsWidth = worldMax.x - worldMin.x;
        float boundsDepth = worldMax.y - worldMin.y;

        float visibleWidth = visibleMax.x - visibleMin.x;

        // Если камера видит больше области, чем существует,
        // центрируем её по соответствующей оси.
        if (visibleWidth >= boundsWidth)
        {
            targetPosition.x +=
                GetBoundsCenter().x - (visibleMin.x + visibleMax.x) * 0.5f;
        }
        else
        {
            if (visibleMin.x < worldMin.x)
            {
                targetPosition.x += worldMin.x - visibleMin.x;
            }

            if (visibleMax.x > worldMax.x)
            {
                targetPosition.x -= visibleMax.x - worldMax.x;
            }
        }

        // После изменения X нужно снова вычислить видимую область,
        // потому что при общем наклоне камеры границы могут быть связаны.
        if (TryGetVisibleRect(targetPosition, out visibleMin, out visibleMax) == false)
        {
            return transform.position;
        }

        float visibleDepth = visibleMax.y - visibleMin.y;

        if (visibleDepth >= boundsDepth)
        {
            targetPosition.z +=
                GetBoundsCenter().y - (visibleMin.y + visibleMax.y) * 0.5f;
        }
        else
        {
            if (visibleMin.y < worldMin.y)
            {
                targetPosition.z += worldMin.y - visibleMin.y;
            }

            if (visibleMax.y > worldMax.y)
            {
                targetPosition.z -= visibleMax.y - worldMax.y;
            }
        }

        targetPosition.y = _fixedCameraY;
        return targetPosition;
    }

    /// <summary>
    /// Возвращает прямоугольник видимой области на плоскости XZ,
    /// где x соответствует мировой оси X, а y — мировой оси Z.
    /// </summary>
    private bool TryGetVisibleRect(Vector3 cameraPosition, out Vector2 min, out Vector2 max)
    {
        min = Vector2.zero;
        max = Vector2.zero;

        if (TryGetVisibleCorners(cameraPosition, _visibleCorners) == false)
        {
            return false;
        }

        min = new Vector2(_visibleCorners[0].x, _visibleCorners[0].z);
        max = min;

        for (int i = 1; i < _visibleCorners.Length; i++)
        {
            Vector3 corner = _visibleCorners[i];

            min = new Vector2(Mathf.Min(min.x, corner.x), Mathf.Min(min.y, corner.z));
            max = new Vector2(Mathf.Max(max.x, corner.x), Mathf.Max(max.y, corner.z));
        }

        return true;
    }

    private bool TryCalculateFitSize(out float size)
    {
        size = _camera.orthographicSize;

        if (TryGetVisibleRect(transform.position, out Vector2 visibleMin, out Vector2 visibleMax) == false)
        {
            return false;
        }

        float visibleWidth = visibleMax.x - visibleMin.x;
        float visibleDepth = visibleMax.y - visibleMin.y;

        if (visibleWidth <= 0f || visibleDepth <= 0f)
        {
            return false;
        }

        float boundsWidth = worldMax.x - worldMin.x;
        float boundsDepth = worldMax.y - worldMin.y;

        // Видимые размеры линейны по orthographicSize при любом наклоне камеры,
        // поэтому достаточно отмасштабировать текущий размер.
        float scale = Mathf.Min(boundsWidth / visibleWidth, boundsDepth / visibleDepth);

        float fitSize = _camera.orthographicSize * scale * FIT_SAFETY_FACTOR;

        size = Mathf.Clamp(fitSize, _minOrthographicSize, _maxOrthographicSize);

        if (Mathf.Approximately(fitSize, size) == false)
        {
            Debug.LogWarning(
                $"Размер камеры упёрся в предел. Желаемый: {fitSize}, применённый: {size}.",
                this);
        }

        return true;
    }

    private bool IsAspectChanged()
    {
        return Mathf.Abs(_camera.aspect - _lastAspect) > ASPECT_CHANGE_TOLERANCE;
    }

    private void ClampCurrentPosition()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = _fixedCameraY;

        Vector3 clampedPosition = ClampPosition(currentPosition);

        if (clampedPosition == transform.position)
        {
            return;
        }

        transform.position = clampedPosition;

        CameraMoved?.Invoke();
    }

    private Vector3 GetVisibleAreaCenter(Vector3 cameraPosition)
    {
        if (!TryGetVisibleCorners(cameraPosition, _visibleCorners))
        {
            return new Vector3(cameraPosition.x, groundY, cameraPosition.z);
        }

        Vector3 center = Vector3.zero;

        for (int i = 0; i < _visibleCorners.Length; i++)
        {
            center += _visibleCorners[i];
        }

        return center / _visibleCorners.Length;
    }

    private Vector2 GetBoundsCenter()
    {
        return (worldMin + worldMax) * 0.5f;
    }

    private void NormalizeBounds()
    {
        Vector2 min = new Vector2(
            Mathf.Min(worldMin.x, worldMax.x),
            Mathf.Min(worldMin.y, worldMax.y)
        );

        Vector2 max = new Vector2(
            Mathf.Max(worldMin.x, worldMax.x),
            Mathf.Max(worldMin.y, worldMax.y)
        );

        worldMin = min;
        worldMax = max;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        NormalizeBounds();

        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        if (_camera != null && _camera.orthographic)
        {
            _fixedCameraY = transform.position.y;
            _fixedCameraRotationX = transform.eulerAngles.x;
            _fixedCameraRotationZ = transform.eulerAngles.z;
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        if (_camera == null || !_camera.orthographic)
        {
            return;
        }

        DrawWorldBounds();

        Vector3 position = transform.position;

        if (!TryGetVisibleCorners(position, _visibleCorners))
        {
            return;
        }

        Gizmos.color = Color.cyan;

        for (int i = 0; i < _visibleCorners.Length; i++)
        {
            Vector3 current = _visibleCorners[i];
            Vector3 next = _visibleCorners[(i + 1) % _visibleCorners.Length];

            Gizmos.DrawLine(current, next);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(
            GetVisibleAreaCenter(position),
            GetVisibleAreaCenter(position) + Vector3.up
        );
    }

    private void DrawWorldBounds()
    {
        Vector3 bottomLeft = new Vector3(worldMin.x, groundY, worldMin.y);
        Vector3 topLeft = new Vector3(worldMin.x, groundY, worldMax.y);
        Vector3 topRight = new Vector3(worldMax.x, groundY, worldMax.y);
        Vector3 bottomRight = new Vector3(worldMax.x, groundY, worldMin.y);

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
#endif
}