using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BuildTowerPopup
{
    [RequireComponent(typeof(Button))]
    public class BuildTowerButton : MonoBehaviour, IView
    {
        public event Action<TowerTypes> Clicked;

        [SerializeField] private TowerTypes _towerType;

        [SerializeField] private Button _button;

        private void OnValidate()
        {
            _button = _button != null ? _button : GetComponent<Button>();
        }
        public void OnInit()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void OnDispose()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(_towerType);
        }
    }
}
