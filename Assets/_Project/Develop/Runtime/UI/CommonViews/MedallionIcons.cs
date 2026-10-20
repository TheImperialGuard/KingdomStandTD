using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Значок медальона по состояниям; у заряженного значок общий — галочка.
    [Serializable]
    public class MedallionIcons
    {
        public MedallionIcons(Sprite idle, Sprite unavailable, Sprite max)
        {
            Idle = idle;
            Unavailable = unavailable;
            Max = max;
        }

        [field: SerializeField] public Sprite Idle { get; private set; }
        [field: SerializeField] public Sprite Unavailable { get; private set; }
        [field: SerializeField] public Sprite Max { get; private set; }
    }
}
