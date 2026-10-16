using Assets._Project.Develop.Runtime.UI.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Признак врага на левой странице: иконка, название и пояснение.
    public class EncyclopediaTraitView : MonoBehaviour, IView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;

        public void Set(Sprite icon, string traitName, string description)
        {
            _icon.sprite = icon;
            _icon.SetNativeSize();
            _name.text = traitName;
            _description.text = description;
        }
    }
}
