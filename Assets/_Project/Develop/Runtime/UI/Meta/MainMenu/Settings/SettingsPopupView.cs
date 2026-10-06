using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Settings;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Settings
{
    public class SettingsPopupView : PopupViewBase
    {
        [SerializeField] private AudioSettingsView _audioSettingsView;

        public AudioSettingsView AudioSettingsView => _audioSettingsView;
    }
}
