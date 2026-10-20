using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerAbilitiesPopup
{
    public class TowerAbilitiesPopupView : TowerPopupViewBase
    {
        public event Action<TowerAbilitiesPopupButtons> ButtonClicked;

        [SerializeField] private TowerMedallionView _firstAbilityButton;
        [SerializeField] private TowerMedallionView _secondAbilityButton;
        [SerializeField] private MedallionButtonView _sellButton;

        // Запечённые состояния значков способностей (Р-39): набор ищется по значку из конфига.
        [SerializeField] private List<MedallionIcons> _abilitiesIcons;

        public void SetAbilityButton(TowerAbilitiesPopupButtons button, MedallionStates state, int cost)
            => GetAbilityButton(button).Set(state, cost);

        public void SetAbilityIcon(TowerAbilitiesPopupButtons button, Sprite icon)
            => GetAbilityButton(button).SetIcons(GetIconsFor(icon));

        public void SetAbilityLevels(TowerAbilitiesPopupButtons button, int level, int maxLevel)
            => GetAbilityButton(button).SetLevels(level, maxLevel);

        public void SetSellButton(MedallionStates state) => _sellButton.SetState(state);

        private void OnEnable()
        {
            _firstAbilityButton.Clicked += OnFirstAbilityButtonClicked;
            _secondAbilityButton.Clicked += OnSecondAbilityButtonClicked;
            _sellButton.Clicked += OnSellButtonClicked;
        }

        private void OnDisable()
        {
            _firstAbilityButton.Clicked -= OnFirstAbilityButtonClicked;
            _secondAbilityButton.Clicked -= OnSecondAbilityButtonClicked;
            _sellButton.Clicked -= OnSellButtonClicked;
        }

        private TowerMedallionView GetAbilityButton(TowerAbilitiesPopupButtons button)
        {
            return button switch
            {
                TowerAbilitiesPopupButtons.FirstAbility => _firstAbilityButton,
                TowerAbilitiesPopupButtons.SecondAbility => _secondAbilityButton,
                _ => throw new ArgumentOutOfRangeException(nameof(button)),
            };
        }

        private MedallionIcons GetIconsFor(Sprite icon)
        {
            foreach (MedallionIcons icons in _abilitiesIcons)
            {
                if (icons.Idle == icon)
                    return icons;
            }

            return new MedallionIcons(icon, icon, icon);
        }

        private void OnFirstAbilityButtonClicked() => ButtonClicked?.Invoke(TowerAbilitiesPopupButtons.FirstAbility);

        private void OnSecondAbilityButtonClicked() => ButtonClicked?.Invoke(TowerAbilitiesPopupButtons.SecondAbility);

        private void OnSellButtonClicked() => ButtonClicked?.Invoke(TowerAbilitiesPopupButtons.Sell);
    }
}
