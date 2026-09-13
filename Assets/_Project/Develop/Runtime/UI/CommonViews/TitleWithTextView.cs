using Assets._Project.Develop.Runtime.UI.Core.Views;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    public class TitleWithTextView : MonoBehaviour, IView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _text;

        public void SetTitle(string text) => _title.text = text;

        public void SetText(string text) => _text.text = text;
    }
}
