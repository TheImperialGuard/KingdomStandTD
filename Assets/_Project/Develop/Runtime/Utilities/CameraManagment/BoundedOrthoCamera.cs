using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class BoundedOrthoCamera : MonoBehaviour
{
    public ReactiveEvent CameraMoved = new();

    [Header("Границы игровой области на плоскости XZ")]
    [SerializeField] private Vector2 worldMin = new(-10f, -10f);
    [SerializeField] private Vector2 worldMax = new(10f, 10f);

    [Header("Плоскость игровой поверхности")]
    [SerializeField] private float groundY = 0f;

    [Header("Минимальный размер области")]
    [SerializeField] private int minResolutionWidth = 1920;
    [SerializeField] private int minResolutionHeight = 1080;

    [Header("Отладка")]
    [SerializeField] private bool drawGizmos = true;

    private Camera _camera;
    private float _fixedCameraY;
    private float _fixedCameraRotationX;
    private float _fixedCameraRotationZ;

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
    }

    private void Start()
    {
        CenterCamera();
    }

    private void LateUpdate()
    {
        // Если ориентация или разрешение изменились,
        // положение камеры необходимо перепроверить.
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
        CenterCamera();
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
        if (!TryGetVisibleCorners(targetPosition, _visibleCorners))
        {
            return transform.position;
        }

        float minVisibleX = _visibleCorners[0].x;
        float maxVisibleX = _visibleCorners[0].x;
        float minVisibleZ = _visibleCorners[0].z;
        float maxVisibleZ = _visibleCorners[0].z;

        for (int i = 1; i < _visibleCorners.Length; i++)
        {
            Vector3 corner = _visibleCorners[i];

            minVisibleX = Mathf.Min(minVisibleX, corner.x);
            maxVisibleX = Mathf.Max(maxVisibleX, corner.x);
            minVisibleZ = Mathf.Min(minVisibleZ, corner.z);
            maxVisibleZ = Mathf.Max(maxVisibleZ, corner.z);
        }

        float visibleWidth = maxVisibleX - minVisibleX;
        float visibleDepth = maxVisibleZ - minVisibleZ;

        float boundsWidth = worldMax.x - worldMin.x;
        float boundsDepth = worldMax.y - worldMin.y;

        // Если камера видит больше области, чем существует,
        // центрируем её по соответствующей оси.
        if (visibleWidth >= boundsWidth)
        {
            targetPosition.x +=
                GetBoundsCenter().x - (minVisibleX + maxVisibleX) * 0.5f;
        }
        else
        {
            if (minVisibleX < worldMin.x)
            {
                targetPosition.x += worldMin.x - minVisibleX;
            }

            if (maxVisibleX > worldMax.x)
            {
                targetPosition.x -= maxVisibleX - worldMax.x;
            }
        }

        // После изменения X нужно снова вычислить видимую область,
        // потому что при общем наклоне камеры границы могут быть связаны.
        if (!TryGetVisibleCorners(targetPosition, _visibleCorners))
        {
            return transform.position;
        }

        minVisibleZ = _visibleCorners[0].z;
        maxVisibleZ = _visibleCorners[0].z;

        for (int i = 1; i < _visibleCorners.Length; i++)
        {
            minVisibleZ = Mathf.Min(minVisibleZ, _visibleCorners[i].z);
            maxVisibleZ = Mathf.Max(maxVisibleZ, _visibleCorners[i].z);
        }

        visibleDepth = maxVisibleZ - minVisibleZ;

        if (visibleDepth >= boundsDepth)
        {
            targetPosition.z +=
                GetBoundsCenter().y - (minVisibleZ + maxVisibleZ) * 0.5f;
        }
        else
        {
            if (minVisibleZ < worldMin.y)
            {
                targetPosition.z += worldMin.y - minVisibleZ;
            }

            if (maxVisibleZ > worldMax.y)
            {
                targetPosition.z -= maxVisibleZ - worldMax.y;
            }
        }

        targetPosition.y = _fixedCameraY;
        return targetPosition;
    }

    private void ClampCurrentPosition()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = _fixedCameraY;

        transform.position = ClampPosition(currentPosition);
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
        worldMin = new Vector2(
            Mathf.Min(worldMin.x, worldMax.x),
            Mathf.Min(worldMin.y, worldMax.y)
        );

        worldMax = new Vector2(
            Mathf.Max(worldMin.x, worldMax.x),
            Mathf.Max(worldMin.y, worldMax.y)
        );
    }

    public bool ValidateBounds()
    {
        float width = worldMax.x - worldMin.x;
        float depth = worldMax.y - worldMin.y;

        float aspect = GetCameraAspect();
        float visibleWorldHeight = _camera.orthographicSize * 2f;
        float visibleWorldWidth = visibleWorldHeight * aspect;

        float minWorldUnitsPerPixel =
            visibleWorldHeight / minResolutionHeight;

        float requiredWidth = minResolutionWidth * minWorldUnitsPerPixel;
        float requiredDepth = minResolutionHeight * minWorldUnitsPerPixel;

        bool valid =
            width >= Mathf.Max(requiredWidth, visibleWorldWidth) &&
            depth >= Mathf.Max(requiredDepth, visibleWorldHeight);

        if (!valid)
        {
            Debug.LogWarning(
                $"Camera bounds are too small.\n" +
                $"Current bounds: {width:F2} x {depth:F2}\n" +
                $"Required approximately: {requiredWidth:F2} x {requiredDepth:F2}",
                this);
        }

        return valid;
    }

    private float GetCameraAspect()
    {
        // Camera.aspect предпочтительнее Screen.width / Screen.height:
        // он учитывает viewport самой камеры.
        return _camera.aspect;
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