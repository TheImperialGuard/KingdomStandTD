using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup
{
    // Плитка характеристики нижней панели: значок, значение и подпись в ряд. Значок и подпись заданы в префабе, нулевая
    // характеристика приглушена.
    public class BottomInfoStatTileView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _caption;

        [SerializeField] private Color _valueColor = new(0.263f, 0.129f, 0.055f);
        [SerializeField] private Color _captionColor = new(0.478f, 0.29f, 0.165f);
        [SerializeField] private Color _mutedTextColor = new(0.698f, 0.612f, 0.463f);
        [SerializeField, Range(0f, 1f)] private float _mutedIconAlpha = 0.45f;

        public void SetVisible(bool isVisible) => gameObject.SetActive(isVisible);

        public void SetIcon(Sprite icon) => _icon.sprite = icon;

        public void SetValue(string value) => _value.text = value;

        public void SetCaption(string caption) => _caption.text = caption;

        public void SetMuted(bool isMuted)
        {
            _icon.color = new Color(1f, 1f, 1f, isMuted ? _mutedIconAlpha : 1f);
            _value.color = isMuted ? _mutedTextColor : _valueColor;
            _caption.color = isMuted ? _mutedTextColor : _captionColor;
        }
    }
}
