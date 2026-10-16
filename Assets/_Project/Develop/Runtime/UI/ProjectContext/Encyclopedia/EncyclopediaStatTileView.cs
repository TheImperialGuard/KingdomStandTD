using Assets._Project.Develop.Runtime.UI.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Плитка характеристики: иконка, число и подпись. Нулевая характеристика приглушена.
    public class EncyclopediaStatTileView : MonoBehaviour, IView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _caption;

        [SerializeField] private Color _mutedTextColor = new(0.698f, 0.612f, 0.463f);
        [SerializeField, Range(0f, 1f)] private float _mutedIconAlpha = 0.45f;

        private Color _valueColor;
        private Color _captionColor;

        private void Awake()
        {
            _valueColor = _value.color;
            _captionColor = _caption.color;
        }

        public void Set(Sprite icon, string value, string caption, bool isMuted)
        {
            _icon.sprite = icon;
            _icon.SetNativeSize();
            _value.text = value;
            _caption.text = caption;

            _icon.color = new Color(1f, 1f, 1f, isMuted ? _mutedIconAlpha : 1f);
            _value.color = isMuted ? _mutedTextColor : _valueColor;
            _caption.color = isMuted ? _mutedTextColor : _captionColor;
        }
    }
}
