using System;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action PlayClicked;
        public event Action EncyclopediaClicked;
        public event Action SettingsClicked;
        public event Action ExitClicked;
        public event Action SizeChanged;

        [SerializeField] private MainMenuItemView _playItem;
        [SerializeField] private MainMenuItemView _encyclopediaItem;
        [SerializeField] private MainMenuItemView _settingsItem;
        [SerializeField] private MainMenuItemView _exitItem;

        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField, Min(0f)] private float _expandedWidth = 432f;
        [SerializeField, Min(0f)] private float _collapsedWidth = 108f;

        private MainMenuItemView[] _items;

        // Корень вью — safe area: SafeAreaContainer подгоняет его под Screen.safeArea.
        public Vector2 AreaSize => ((RectTransform)transform).rect.size;

        private void Awake()
        {
            _items = new[] { _playItem, _encyclopediaItem, _settingsItem, _exitItem };
        }

        private void OnEnable()
        {
            _playItem.Clicked += OnPlayItemClicked;
            _encyclopediaItem.Clicked += OnEncyclopediaItemClicked;
            _settingsItem.Clicked += OnSettingsItemClicked;
            _exitItem.Clicked += OnExitItemClicked;
        }

        private void OnDisable()
        {
            _playItem.Clicked -= OnPlayItemClicked;
            _encyclopediaItem.Clicked -= OnEncyclopediaItemClicked;
            _settingsItem.Clicked -= OnSettingsItemClicked;
            _exitItem.Clicked -= OnExitItemClicked;
        }

        private void OnRectTransformDimensionsChange() => SizeChanged?.Invoke();

        public void SetSelectedItem(MainMenuPanels panel)
        {
            _playItem.SetSelected(panel == MainMenuPanels.Play);
            _encyclopediaItem.SetSelected(panel == MainMenuPanels.Encyclopedia);
            _settingsItem.SetSelected(panel == MainMenuPanels.Settings);
        }

        public void SetCollapsed(bool isCollapsed)
        {
            float width = isCollapsed ? _collapsedWidth : _expandedWidth;
            _itemsContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            foreach (MainMenuItemView item in _items)
                item.SetCollapsed(isCollapsed);
        }

        private void OnPlayItemClicked() => PlayClicked?.Invoke();

        private void OnEncyclopediaItemClicked() => EncyclopediaClicked?.Invoke();

        private void OnSettingsItemClicked() => SettingsClicked?.Invoke();

        private void OnExitItemClicked() => ExitClicked?.Invoke();
    }
}
