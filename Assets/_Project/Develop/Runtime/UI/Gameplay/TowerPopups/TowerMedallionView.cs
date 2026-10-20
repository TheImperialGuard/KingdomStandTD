using Assets._Project.Develop.Runtime.UI.CommonViews;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups
{
    // Кнопка окна башни с ценой: медальон, ценник под ним и кружки уровней над ним (только у способностей).
    public class TowerMedallionView : MonoBehaviour
    {
        public event Action Clicked;

        [SerializeField] private MedallionButtonView _button;
        [SerializeField] private MedallionPriceView _price;
        [SerializeField] private MedallionLevelsView _levels;

        public void Set(MedallionStates state, int cost)
        {
            _button.SetState(state);

            if (state == MedallionStates.Max)
                _price.SetMax();
            else
                _price.SetCost(cost, state != MedallionStates.Unavailable);
        }

        public void SetIcons(MedallionIcons icons) => _button.SetIcons(icons);

        public void SetLevels(int level, int maxLevel) => _levels.Set(level, maxLevel);

        private void OnEnable() => _button.Clicked += OnButtonClicked;

        private void OnDisable() => _button.Clicked -= OnButtonClicked;

        private void OnButtonClicked() => Clicked?.Invoke();
    }
}
