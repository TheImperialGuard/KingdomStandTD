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

        private readonly List<RaycastResult> _uiRaycastResults = new();
        private readonly float _dragThresholdPixels;

        private PointerEventData _uiPointerEventData;
        private Vector2 _touchStartPosition;
        private Vector2 _tapPosition;
        private Vector3 _cameraDelta;
        private int _processedFrame = -1;
        private int _trackedFingerId = NO_FINGER_ID;
        private bool _isDragging;
        private bool _isStartedOverUI;
        private bool _isTapPerformed;

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

        // Состояние пересчитывается один раз за кадр: потребители опрашивают
        // свойства по нескольку раз, а разбор касаний не идемпотентен.
        private void UpdateState()
        {
            if (_processedFrame == Time.frameCount)
                return;

            _processedFrame = Time.frameCount;
            _isTapPerformed = false;
            _cameraDelta = Vector3.zero;

            if (Input.touchCount == 0)
            {
                ResetTracking();
                return;
            }

            // Мультитач тапом быть не может.
            if (Input.touchCount > 1)
                _isDragging = true;

            for (int i = 0; i < Input.touchCount; i++)
                ProcessTouch(Input.GetTouch(i));
        }

        private void ProcessTouch(Touch touch)
        {
            if (_trackedFingerId == NO_FINGER_ID && touch.phase == TouchPhase.Began)
            {
                BeginTouch(touch);
                return;
            }

            if (touch.fingerId != _trackedFingerId)
                return;

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
