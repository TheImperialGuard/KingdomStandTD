using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Спрайты лица медальона по состояниям. Наведение запечено отдельно (brightness 1.12 макета); у недоступного и
    // максимума наведения нет.
    [Serializable]
    public class MedallionFaces
    {
        [field: SerializeField] public Sprite Idle { get; private set; }
        [field: SerializeField] public Sprite IdleHovered { get; private set; }
        [field: SerializeField] public Sprite Armed { get; private set; }
        [field: SerializeField] public Sprite ArmedHovered { get; private set; }
        [field: SerializeField] public Sprite Unavailable { get; private set; }
        [field: SerializeField] public Sprite Max { get; private set; }
    }
}
