using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Левая страница любого раздела: картинка в золотой рамке, заголовок, под ним признаки врага или описание башни.
    // Пока картинки у записи нет, в рамке заглушка — штриховка, значок раздела и подпись (Р-18).
    public class EncyclopediaLeftPageView : MonoBehaviour
    {
        [SerializeField] private Image _picture;
        [SerializeField] private GameObject _placeholder;
        [SerializeField] private Image _placeholderIcon;
        [SerializeField] private TMP_Text _placeholderCaption;
        [SerializeField, Min(0f)] private float _placeholderIconScale = 0.5f;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private EncyclopediaTraitsListView _traitsList;
        [SerializeField] private TMP_Text _description;

        public EncyclopediaTraitsListView TraitsList => _traitsList;

        public void SetPlaceholder(Sprite icon, string caption)
        {
            _placeholderIcon.sprite = icon;
            _placeholderIcon.SetNativeSize();
            _placeholderIcon.rectTransform.sizeDelta *= _placeholderIconScale;
            _placeholderCaption.text = caption;
        }

        public void SetPicture(Sprite picture)
        {
            bool hasPicture = picture != null;

            _picture.sprite = picture;
            _picture.gameObject.SetActive(hasPicture);
            _placeholder.SetActive(hasPicture == false);
        }

        public void SetTitle(string title) => _title.text = title;

        public void SetDescription(string description)
        {
            _description.text = description;
            _description.gameObject.SetActive(string.IsNullOrEmpty(description) == false);
        }

        public void SetTraitsVisible(bool isVisible) => _traitsList.gameObject.SetActive(isVisible);
    }
}
