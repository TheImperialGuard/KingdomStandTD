using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Settings
{
    // Ряд одного канала звука в окне настроек меню (Р-61): выключенный ряд — перечёркнутая иконка и бледный ползунок.
    public class AudioChannelView : MonoBehaviour
    {
        private const float OffSliderAlpha = 0.45f;

        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _onIcon;
        [SerializeField] private Sprite _offIcon;
        [SerializeField] private CanvasGroup _sliderGroup;

        public void SetOn(bool isOn)
        {
            _icon.sprite = isOn ? _onIcon : _offIcon;
            _sliderGroup.alpha = isOn ? 1f : OffSliderAlpha;
        }
    }
}
