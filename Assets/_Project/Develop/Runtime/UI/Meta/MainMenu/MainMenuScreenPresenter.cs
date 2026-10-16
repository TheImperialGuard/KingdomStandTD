using System;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    // Левое меню и правые панели: открыта одна панель за раз (Р-11). Переход к цели — закрыть открытую панель и по окончании
    // открыть цель; щелчок во время перехода меняет цель (Р-24).
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _view;
        private readonly MainMenuPopupService _popupService;
        private readonly HotkeysService _hotkeysService;

        private IDisposable _closeRequestedDisposable;

        private MainMenuPanels _openedPanel = MainMenuPanels.None;
        private MainMenuPanels _targetPanel = MainMenuPanels.None;
        private PopupPresenterBase _openedPopup;
        private bool _isClosing;

        public MainMenuScreenPresenter(
            MainMenuScreenView view,
            MainMenuPopupService popupService,
            HotkeysService hotkeysService)
        {
            _view = view;
            _popupService = popupService;
            _hotkeysService = hotkeysService;
        }

        public void Initialize()
        {
            _view.PlayClicked += OnPlayClicked;
            _view.EncyclopediaClicked += OnEncyclopediaClicked;
            _view.SettingsClicked += OnSettingsClicked;
            _view.ExitClicked += OnExitClicked;
            _view.SizeChanged += OnSizeChanged;

            _closeRequestedDisposable = _hotkeysService.CloseRequested.Subscribe(OnCloseRequested);

            UpdateLayout();
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
            _view.EncyclopediaClicked -= OnEncyclopediaClicked;
            _view.SettingsClicked -= OnSettingsClicked;
            _view.ExitClicked -= OnExitClicked;
            _view.SizeChanged -= OnSizeChanged;

            _closeRequestedDisposable.Dispose();
        }

        public void OpenPanel(MainMenuPanels panel) => SetTarget(panel);

        private void OnPlayClicked() => ToggleTarget(MainMenuPanels.Play);

        private void OnEncyclopediaClicked() => Debug.Log("Энциклопедия ещё не подключена к меню");

        private void OnSettingsClicked() => ToggleTarget(MainMenuPanels.Settings);

        private void OnExitClicked() => Quit();

        private void OnCloseRequested() => SetTarget(MainMenuPanels.None);

        private void OnSizeChanged() => UpdateLayout();

        private void ToggleTarget(MainMenuPanels panel)
            => SetTarget(_targetPanel == panel ? MainMenuPanels.None : panel);

        // Выбранный пункт переходит к цели сразу, не дожидаясь закрытия открытой панели (Р-51).
        private void SetTarget(MainMenuPanels panel)
        {
            _targetPanel = panel;
            _view.SetSelectedItem(panel);

            ProcessTransition();
            UpdateLayout();
        }

        private void ProcessTransition()
        {
            if (_isClosing || _openedPanel == _targetPanel)
                return;

            if (_openedPanel != MainMenuPanels.None)
            {
                _isClosing = true;
                _popupService.ClosePopup(_openedPopup);
                return;
            }

            _openedPanel = _targetPanel;
            _openedPopup = OpenPopup(_targetPanel);
        }

        private PopupPresenterBase OpenPopup(MainMenuPanels panel)
        {
            switch (panel)
            {
                case MainMenuPanels.Play:
                    return _popupService.OpenPlayBoardPopup(OnPopupClosed);

                case MainMenuPanels.Settings:
                    return _popupService.OpenSettingsPopup(OnPopupClosed);

                default:
                    throw new ArgumentOutOfRangeException(nameof(panel), panel, null);
            }
        }

        // Зовётся и после закрытия по просьбе презентера, и когда панель закрылась сама — тогда цель сбрасывается.
        private void OnPopupClosed()
        {
            if (_isClosing == false)
            {
                _targetPanel = MainMenuPanels.None;
                _view.SetSelectedItem(MainMenuPanels.None);
            }

            _isClosing = false;
            _openedPanel = MainMenuPanels.None;
            _openedPopup = null;

            ProcessTransition();
            UpdateLayout();
        }

        // Меню сворачивается с началом открытия панели и разворачивается, когда панель закрылась и следующая не
        // открывается (Р-51): раскладка считается по открытой панели, а не по цели.
        private void UpdateLayout()
        {
            MainMenuLayout layout = MainMenuLayout.Calculate(_view.AreaSize, _openedPanel);
            _view.SetCollapsed(layout.IsMenuCollapsed);
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
