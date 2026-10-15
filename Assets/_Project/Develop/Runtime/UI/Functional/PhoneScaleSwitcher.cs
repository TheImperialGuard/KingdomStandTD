using Assets._Project.Develop.Runtime.Utilities.Browser;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    // На телефоне делает интерфейс крупнее: уменьшает высоту эталона у CanvasScaler, масштабирующего по высоте.
    // Телефон — логическая высота игровой области ниже порога: в браузере в CSS-точках, в редакторе в пикселях окна Game.
    [RequireComponent(typeof(CanvasScaler))]
    public class PhoneScaleSwitcher : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _phoneHeightThreshold = 480;
        [SerializeField, Min(1)] private int _defaultReferenceHeight = 1080;
        [SerializeField, Min(1)] private int _phoneReferenceHeight = 960;

        private CanvasScaler _canvasScaler;

        public int LogicalHeight { get; private set; }

        public bool IsPhone { get; private set; }

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            UpdateMode();
            ApplyReferenceHeight();
        }

        // Корневой канвас меняет размер вместе с экраном, поэтому своего Update не нужно. Смена размера окна без смены
        // пропорций сюда не доходит: у канваса, масштабируемого по высоте, размер в единицах канваса не меняется.
        private void OnRectTransformDimensionsChange()
        {
            // Сообщение может прийти раньше Awake — тогда пересчитывать ещё нечего.
            if (_canvasScaler == null)
                return;

            bool wasPhone = IsPhone;
            UpdateMode();

            if (IsPhone != wasPhone)
                ApplyReferenceHeight();
        }

        private void UpdateMode()
        {
            LogicalHeight = BrowserScreen.LogicalHeight;
            IsPhone = LogicalHeight < _phoneHeightThreshold;
        }

        private void ApplyReferenceHeight()
        {
            int referenceHeight = IsPhone ? _phoneReferenceHeight : _defaultReferenceHeight;
            _canvasScaler.referenceResolution = new Vector2(_canvasScaler.referenceResolution.x, referenceHeight);
        }
    }
}
