using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class MobileInput : IInputService
    {
        private const float DRAG_THRESHOLD_INCHES = 0.06f;
        private const float FALLBACK_SCREEN_DPI = 160f;
        private const float MIN_DRAG_THRESHOLD_PIXELS = 8f;
        private const float MIN_PITCH_SIN = 0.05f;
        private const int NO_FINGER_ID = -1;
        private const float NoZoom = 1f;

        private readonly List<RaycastResult> _uiRaycastResults = new();
        private readonly float _dragThresholdPixels;

        private PointerEventData _uiPointerEventData;
        private Vector2 _touchStartPosition;
        private Vector2 _tapPosition;
        private Vector2 _cameraZoomPivot;
        private Vector3 _cameraDelta;
        private float _cameraZoomFactor = NoZoom;
        private int _processedFrame = -1;
        private int _trackedFingerId = NO_FINGER_ID;
        private bool _isDragging;
        private bool _isStartedOverUI;
        private bool _isTapPerformed;
        private bool _isPinchBlocked;

        public MobileInput()
        {
            float screenDpi = Screen.dpi > 0f ? Screen.dpi : FALLBACK_SCREEN_DPI;

            _dragThresholdPixels = Mathf.Max(screenDpi * DRAG_THRESHOLD_INCHES, MIN_DRAG_THRESHOLD_PIXELS);
        }

        public bool IsEnabled { get; set; } = true;

        public bool RayShotRequested
        {
            get
            {
                UpdateState();

                return IsEnabled && _isTapPerformed;
            }
        }

        // Клавиши Esc на телефоне в браузере нет.
        public bool CloseRequested => false;

        // Стрелок тоже нет: книгу листают её кнопками.
        public bool PreviousRequested => false;

        public bool NextRequested => false;

        public Ray CameraRay
        {
            get
            {
                Camera camera = Camera.main;

                if (camera == null)
                    return new Ray();

                return camera.ScreenPointToRay(_tapPosition);
            }
        }

        public Vector3 CameraDelta
        {
            get
            {
                UpdateState();

                if (IsEnabled == false)
                    return Vector3.zero;

                return _cameraDelta;
            }
        }

        public float CameraZoomFactor
        {
            get
            {
                UpdateState();

                if (IsEnabled == false)
                    return NoZoom;

                return _cameraZoomFactor;
            }
        }

        public Vector2 CameraZoomPivot
        {
            get
            {
                UpdateState();

                return _cameraZoomPivot;
            }
        }

        // Состояние пересчитывается один раз за кадр: потребители опрашивают
        // свойства по нескольку раз, а разбор касаний не идемпотентен.
        private void UpdateState()
        {
            if (_processedFrame == Time.frameCount)
                return;

            _processedFrame = Time.frameCount;
            _isTapPerformed = false;
            _cameraDelta = Vector3.zero;
            _cameraZoomFactor = NoZoom;

            if (Input.touchCount == 0)
            {
                ResetTracking();
                _isPinchBlocked = false;
                return;
            }

            // Мультитач тапом быть не может.
            if (Input.touchCount > 1)
                _isDragging = true;

            for (int i = 0; i < Input.touchCount; i++)
                ProcessTouch(Input.GetTouch(i));

            if (Input.touchCount > 1)
                UpdatePinch(Input.GetTouch(0), Input.GetTouch(1));
        }

        private void ProcessTouch(Touch touch)
        {
            if (_trackedFingerId == NO_FINGER_ID && touch.phase == TouchPhase.Began)
            {
                BeginTouch(touch);
                return;
            }

            if (touch.fingerId != _trackedFingerId)
            {
                // Второй палец на интерфейсе — например, нажатие кнопки, пока первый двигает карту, — щипком не считается.
                if (touch.phase == TouchPhase.Began && IsPointerOverUI(touch.position))
                    _isPinchBlocked = true;

                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    UpdateDrag(touch);
                    break;

                case TouchPhase.Ended:
                    EndTouch(touch);
                    break;

                case TouchPhase.Canceled:
                    ResetTracking();
                    break;
            }
        }

        private void BeginTouch(Touch touch)
        {
            _trackedFingerId = touch.fingerId;
            _touchStartPosition = touch.position;
            _isDragging = false;
            _isStartedOverUI = IsPointerOverUI(touch.position);
        }

        private void UpdateDrag(Touch touch)
        {
            // Смещение считается от точки старта, а не накоплением пути:
            // дрожание неподвижного пальца иначе превратит долгий тап в свайп.
            if (_isDragging == false && Vector2.Distance(touch.position, _touchStartPosition) >= _dragThresholdPixels)
                _isDragging = true;

            if (_isDragging && _isStartedOverUI == false)
                _cameraDelta = ToWorldDelta(touch.deltaPosition);
        }

        private void EndTouch(Touch touch)
        {
            // Тап засчитывается на отрыве пальца, иначе каждый свайп начинался бы
            // с выстрела луча в мир.
            if (_isDragging == false && _isStartedOverUI == false)
            {
                _isTapPerformed = true;
                _tapPosition = touch.position;
            }

            ResetTracking();
        }

        // Два пальца двигают карту за серединой между ними и меняют масштаб вокруг неё. Палец, который только лёг
        // или уже отрывается, в кадре не учитывается: так карта не прыгает в начале и в конце жеста.
        private void UpdatePinch(Touch first, Touch second)
        {
            _cameraDelta = Vector3.zero;

            if (_isStartedOverUI || _isPinchBlocked || IsMoving(first) == false || IsMoving(second) == false)
                return;

            Vector2 previousFirst = first.position - first.deltaPosition;
            Vector2 previousSecond = second.position - second.deltaPosition;

            Vector2 middle = (first.position + second.position) / 2f;
            Vector2 previousMiddle = (previousFirst + previousSecond) / 2f;

            float distance = Vector2.Distance(first.position, second.position);
            float previousDistance = Vector2.Distance(previousFirst, previousSecond);

            _cameraDelta = ToWorldDelta(middle - previousMiddle);
            _cameraZoomPivot = middle;

            if (distance > 0f && previousDistance > 0f)
                _cameraZoomFactor = previousDistance / distance;
        }

        private bool IsMoving(Touch touch) => touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;

        private void ResetTracking()
        {
            _trackedFingerId = NO_FINGER_ID;
            _isDragging = false;
            _isStartedOverUI = false;
        }

        // Своя проверка вместо IsPointerOverGameObject: порядок Update между
        // EventSystem и бутстрапом не задан, и на кадре касания указатель
        // модуля может быть ещё не заведён или уже освобождён.
        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null)
                return false;

            if (_uiPointerEventData == null)
                _uiPointerEventData = new PointerEventData(eventSystem);

            _uiPointerEventData.position = screenPosition;

            _uiRaycastResults.Clear();
            eventSystem.RaycastAll(_uiPointerEventData, _uiRaycastResults);

            return _uiRaycastResults.Count > 0;
        }

        private Vector3 ToWorldDelta(Vector2 screenDelta)
        {
            Camera camera = Camera.main;

            if (camera == null)
                return Vector3.zero;

            float worldUnitsPerPixel = camera.orthographicSize * 2f / camera.pixelHeight;

            // Наклон камеры растягивает вертикаль экрана вдоль оси Z.
            float pitchSin = Mathf.Max(Mathf.Sin(camera.transform.eulerAngles.x * Mathf.Deg2Rad), MIN_PITCH_SIN);

            return new Vector3(
                -screenDelta.x * worldUnitsPerPixel,
                0f,
                -screenDelta.y * worldUnitsPerPixel / pitchSin);
        }
    }
}
