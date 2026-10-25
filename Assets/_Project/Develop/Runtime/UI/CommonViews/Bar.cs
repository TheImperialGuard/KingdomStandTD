using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Заливка — Image с типом Filled, без маски: маска рвёт батч Canvas, а части полоски с одной текстурой атласа
    // идут одним вызовом отрисовки
    public class Bar : MonoBehaviour, IView
    {
        [SerializeField] private Image _filler;

        public void UpdateValue(float value) => _filler.fillAmount = value;

        public void SetFillerColor(Color color) => _filler.color = color;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
