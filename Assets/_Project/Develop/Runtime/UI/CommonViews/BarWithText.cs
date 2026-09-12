using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    public class BarWithText : MonoBehaviour, IView
    {
        [SerializeField] private Bar _bar;

        [SerializeField] private TMP_Text _text;

        public void UpdateText(string text) => _text.text = text;

        public void UpdateSliderValue(float value) => _bar.UpdateSliderValue(value);

        public void SetFillerColor(Color color) => _bar.SetFillerColor(color);
    }
}
