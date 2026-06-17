using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities
{
    public class Layers
    {
        public static readonly int Triggers = LayerMask.NameToLayer("Triggers");
        public static readonly LayerMask TriggersMask = 1 << Triggers;
    }
}
