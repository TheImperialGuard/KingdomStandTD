using Assets._Project.Develop.Runtime.UI.CommonViews;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups
{
    // Боковая подсказка окна башни на пергаменте: название, подзаголовок, описание, параметры и большое число продажи.
    // Ряд с пустой строкой прячется вместе с отступом над ним. Место ставит окно — по размеру подсказки после смены текста.
    public class TowerTipView : MonoBehaviour
    {
        [SerializeField] private ParchmentTipView _parchment;
        [SerializeField] private RectTransform _body;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private GameObject _subtitleRow;
        [SerializeField] private TMP_Text _subtitle;
        [SerializeField] private GameObject _descriptionRow;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private GameObject _parametersRow;
        [SerializeField] private TMP_Text _parameters;
        [SerializeField] private GameObject _bigNumberRow;
        [SerializeField] private TMP_Text _bigNumber;

        public bool IsShown => gameObject.activeSelf;

        public Vector2 Size => _body.rect.size;

        public void Show(TowerTipContent content)
        {
            _title.text = content.Title;

            SetRow(_subtitleRow, _subtitle, content.Subtitle);
            SetRow(_descriptionRow, _description, content.Description);
            SetRow(_parametersRow, _parameters, content.Parameters);
            SetRow(_bigNumberRow, _bigNumber, content.BigNumber);

            _parchment.Show();
        }

        public void Hide() => _parchment.Hide();

        private static void SetRow(GameObject row, TMP_Text text, string value)
        {
            bool hasValue = string.IsNullOrEmpty(value) == false;

            row.SetActive(hasValue);

            if (hasValue)
                text.text = value;
        }
    }
}
