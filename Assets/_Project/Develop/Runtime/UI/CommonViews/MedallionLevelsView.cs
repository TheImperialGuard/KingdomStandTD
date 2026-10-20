using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Кружки уровней над медальоном: по одному на уровень, закрашены достигнутые. Лишние кружки прячутся.
    public class MedallionLevelsView : MonoBehaviour
    {
        [SerializeField] private List<Image> _pips;
        [SerializeField] private Sprite _reachedSprite;
        [SerializeField] private Sprite _notReachedSprite;

        public void Set(int level, int maxLevel)
        {
            gameObject.SetActive(true);

            for (int i = 0; i < _pips.Count; i++)
            {
                _pips[i].gameObject.SetActive(i < maxLevel);
                _pips[i].sprite = i < level ? _reachedSprite : _notReachedSprite;
            }
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
