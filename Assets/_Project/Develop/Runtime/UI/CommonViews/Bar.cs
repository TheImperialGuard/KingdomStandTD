using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    public class Bar : MonoBehaviour, IView
    {
        [SerializeField] private Slider _slider;

        [SerializeField] private Image _filler;

        public void UpdateSliderValue(float value) => _slider.value = value;

        public void SetFillerColor(Color color) => _filler.color = color;

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
