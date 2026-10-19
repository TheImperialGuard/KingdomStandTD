using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Settings;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Settings
{
    // Окно «Настройки» — доска на цепях рядом с левым меню (Р-63, spec-settings.md).
    public class SettingsPopupView : ChainBoardPopupView
    {
        [SerializeField] private AudioSettingsView _audioSettingsView;

        public AudioSettingsView AudioSettingsView => _audioSettingsView;
    }
}
