using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    public class LevelPosition : MonoBehaviour
    {
        [field: SerializeField] public List<Transform> PathPointViews { get; private set; }

        public RectTransform Position => GetComponent<RectTransform>();
    }
}
