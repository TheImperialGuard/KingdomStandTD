using Assets._Project.Develop.Runtime.Utilities.Browser;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    // На телефоне делает интерфейс крупнее: уменьшает эталон у CanvasScaler в режиме Expand. Ширина эталона — самое узкое
    // окно, в котором раскладка помещается целиком: шире масштаб идёт по высоте, уже — по ширине (Р-78). Числа эталона и
    // признак телефона — MenuCanvasScale: по ним же пауза боя считает масштаб меню.
    [RequireComponent(typeof(CanvasScaler))]
    public class PhoneScaleSwitcher : MonoBehaviour
    {
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
        // пропорций сюда не доходит: размер канваса в единицах эталона при этом не меняется.
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
            IsPhone = MenuCanvasScale.IsPhone(LogicalHeight);
        }

        private void ApplyReferenceHeight() => _canvasScaler.referenceResolution = MenuCanvasScale.GetReference(IsPhone);
    }
}
