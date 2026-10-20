using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Кнопка-медальон окон башни и кнопки волны: лицо и значок по состоянию. Наведение — Sprite Swap кнопки, нажатие —
    // PointerFeedbackView. Ценник и кружки уровней — отдельные вью, детьми Press.
    public class MedallionButtonView : MonoBehaviour
    {
        public event Action Clicked;

        [SerializeField] private Button _button;
        [SerializeField] private PointerFeedbackView _pointerFeedback;
        [SerializeField] private Image _face;
        [SerializeField] private Image _icon;

        [SerializeField] private MedallionFaces _faces;
        [SerializeField] private MedallionIcons _icons;
        [SerializeField] private Sprite _armedIcon;

        private MedallionStates _state;

        public MedallionStates State => _state;

        public void SetState(MedallionStates state)
        {
            _state = state;
            ApplyState();
        }

        public void SetFaces(MedallionFaces faces)
        {
            _faces = faces;
            ApplyState();
        }

        public void SetIcons(MedallionIcons icons)
        {
            _icons = icons;
            ApplyState();
        }

        private void Awake() => ApplyState();

        private void OnEnable() => _button.onClick.AddListener(OnButtonClicked);

        private void OnDisable() => _button.onClick.RemoveListener(OnButtonClicked);

        private void ApplyState()
        {
            Sprite hovered = GetHoveredFace();

            // Sprite Swap перекрашивает лицо только при смене состояния кнопки. Если лицо сейчас подменено наведением,
            // подмену обновляем сами — иначе до ухода указателя осталось бы лицо прежнего состояния.
            bool isSwapped = _face.overrideSprite != _face.sprite;

            _face.sprite = GetFace();

            if (isSwapped)
                _face.overrideSprite = hovered;

            _button.spriteState = new SpriteState
            {
                highlightedSprite = hovered,
                pressedSprite = hovered,
            };

            _icon.sprite = GetIcon();
            _icon.SetNativeSize();

            _button.interactable = _state == MedallionStates.Idle || _state == MedallionStates.Armed;
            _pointerFeedback.Refresh();
        }

        private Sprite GetFace()
        {
            switch (_state)
            {
                case MedallionStates.Idle:
                    return _faces.Idle;

                case MedallionStates.Armed:
                    return _faces.Armed;

                case MedallionStates.Unavailable:
                    return _faces.Unavailable;

                case MedallionStates.Max:
                    return _faces.Max;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state));
            }
        }

        private Sprite GetHoveredFace()
        {
            switch (_state)
            {
                case MedallionStates.Idle:
                    return _faces.IdleHovered;

                case MedallionStates.Armed:
                    return _faces.ArmedHovered;

                default:
                    return null;
            }
        }

        private Sprite GetIcon()
        {
            switch (_state)
            {
                case MedallionStates.Idle:
                    return _icons.Idle;

                case MedallionStates.Armed:
                    return _armedIcon;

                case MedallionStates.Unavailable:
                    return _icons.Unavailable;

                case MedallionStates.Max:
                    return _icons.Max;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state));
            }
        }

        private void OnButtonClicked() => Clicked?.Invoke();
    }
}
