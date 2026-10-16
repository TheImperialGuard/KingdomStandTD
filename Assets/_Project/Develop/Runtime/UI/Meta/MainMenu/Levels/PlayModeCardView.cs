using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    // Карточка-знамя режима игры. При наведении — тёмная подложка с серебряной рамкой, знамя приподнимается и качается,
    // заголовок крупнее; при нажатии знамя опускается на место. Приглушение соседней карточки — у вью доски. Не Selectable:
    // навигации и залипшего выделения нет (Р-25).
    public class PlayModeCardView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
    {
        public event Action Clicked;
        public event Action<PlayModeCardView> HoverStarted;
        public event Action<PlayModeCardView> HoverEnded;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _back;
        [SerializeField] private GameObject _edges;
        [SerializeField] private RectTransform _lift;
        [SerializeField] private RectTransform _flag;
        [SerializeField] private RectTransform _title;

        [SerializeField, Range(0f, 1f)] private float _backHoveredAlpha = 0.58f;
        [SerializeField, Range(0f, 1f)] private float _dimmedAlpha = 0.7f;
        [SerializeField] private float _hoveredLift = 9f;
        [SerializeField, Min(1f)] private float _hoveredTitleScale = 1.08f;
        [SerializeField, Min(0f)] private float _duration = 0.15f;
        [SerializeField] private float _swayAngle = 2.5f;
        [SerializeField, Min(0.01f)] private float _swayStepDuration = 0.4f;

        private Vector2 _liftIdlePosition;

        private Tween _liftTween;
        private Tween _titleTween;
        private Tween _dimTween;
        private Sequence _sway;

        private bool _isHovered;
        private bool _isPressed;

        private void Awake()
        {
            _liftIdlePosition = _lift.anchoredPosition;
            UpdateHighlight();
        }

        private void OnDisable()
        {
            _isHovered = false;
            _isPressed = false;

            KillTweens();
            _lift.anchoredPosition = _liftIdlePosition;
            _title.localScale = Vector3.one;
            _canvasGroup.alpha = 1f;
            UpdateHighlight();
        }

        private void OnDestroy() => KillTweens();

        public void SetDimmed(bool isDimmed)
        {
            _dimTween?.Kill();

            _dimTween = _canvasGroup
                .DOFade(isDimmed ? _dimmedAlpha : 1f, _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateHighlight();
            HoverStarted?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            _isPressed = false;
            UpdateHighlight();
            HoverEnded?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPressed = true;
            UpdateLift();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPressed = false;
            UpdateLift();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                Clicked?.Invoke();
        }

        private void UpdateHighlight()
        {
            Color backColor = _back.color;
            backColor.a = _isHovered ? _backHoveredAlpha : 0f;
            _back.color = backColor;

            _edges.SetActive(_isHovered);

            UpdateLift();
            UpdateTitle();
            UpdateSway();
        }

        private void UpdateLift()
        {
            Vector2 position = _isHovered && _isPressed == false
                ? _liftIdlePosition + Vector2.up * _hoveredLift
                : _liftIdlePosition;

            _liftTween?.Kill();

            _liftTween = _lift
                .DOAnchorPos(position, _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        private void UpdateTitle()
        {
            _titleTween?.Kill();

            _titleTween = _title
                .DOScale(_isHovered ? _hoveredTitleScale : 1f, _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        // Знамя переступает ступенькой между двумя наклонами, как steps(1, end) в макете.
        private void UpdateSway()
        {
            _sway?.Kill();
            SetFlagAngle(0f);

            if (_isHovered == false)
                return;

            SetFlagAngle(-_swayAngle);

            _sway = DOTween.Sequence()
                .AppendInterval(_swayStepDuration)
                .AppendCallback(() => SetFlagAngle(_swayAngle))
                .AppendInterval(_swayStepDuration)
                .AppendCallback(() => SetFlagAngle(-_swayAngle))
                .SetLoops(-1)
                .SetUpdate(true);
        }

        // В макете rotate по часовой стрелке, в Unity положительный угол — против неё.
        private void SetFlagAngle(float clockwiseAngle) => _flag.localRotation = Quaternion.Euler(0f, 0f, -clockwiseAngle);

        private void KillTweens()
        {
            _liftTween?.Kill();
            _titleTween?.Kill();
            _dimTween?.Kill();
            _sway?.Kill();
        }
    }
}
