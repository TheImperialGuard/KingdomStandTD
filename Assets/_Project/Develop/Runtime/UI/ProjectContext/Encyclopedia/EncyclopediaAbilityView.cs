using Assets._Project.Develop.Runtime.UI.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Способность на правой странице: иконка на плашке, название, описание и строка цифр цветом раздела.
    public class EncyclopediaAbilityView : MonoBehaviour, IView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _numbers;

        public void Set(Sprite icon, string abilityName, string description, string numbers)
        {
            _icon.sprite = icon;
            _icon.enabled = icon != null;

            if (icon != null)
                _icon.SetNativeSize();

            _name.text = abilityName;
            _description.text = description;
            _numbers.text = numbers;
            _numbers.gameObject.SetActive(string.IsNullOrEmpty(numbers) == false);
        }

        public void SetNumbersColor(Color color) => _numbers.color = color;
    }
}
