using Assets._Project.Develop.Runtime.UI.Core.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public List<RectTransform> LevelsPositionsList { get; private set; }
    }
}
