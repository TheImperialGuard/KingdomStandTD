using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Правая страница врага или башни: «Характеристики» плитками и «Способности» на плашках. Заголовки блоков, линии под
    // ними и строки цифр способностей — цветом раздела.
    public class EncyclopediaEntityPageView : MonoBehaviour
    {
        [SerializeField] private EncyclopediaStatTilesListView _statTilesList;
        [SerializeField] private GameObject _abilitiesBlock;
        [SerializeField] private EncyclopediaAbilitiesListView _abilitiesList;
        [SerializeField] private List<TMP_Text> _headers;
        [SerializeField] private List<Image> _headerLines;

        private Color _accentDarkColor;

        public EncyclopediaStatTilesListView StatTilesList => _statTilesList;

        public IReadOnlyList<EncyclopediaAbilityView> Abilities => _abilitiesList.Elements;

        public void SetAccentColors(Color accentColor, Color accentDarkColor)
        {
            _accentDarkColor = accentDarkColor;

            foreach (TMP_Text header in _headers)
                header.color = accentDarkColor;

            foreach (Image line in _headerLines)
                line.color = accentColor;

            foreach (EncyclopediaAbilityView ability in _abilitiesList.Elements)
                ability.SetNumbersColor(accentDarkColor);
        }

        public void AddAbility(EncyclopediaAbilityView ability)
        {
            _abilitiesList.AddElement(ability);
            ability.SetNumbersColor(_accentDarkColor);
        }

        public void RemoveAbility(EncyclopediaAbilityView ability) => _abilitiesList.RemoveElement(ability);

        public void SetAbilitiesVisible(bool isVisible) => _abilitiesBlock.SetActive(isVisible);
    }
}
