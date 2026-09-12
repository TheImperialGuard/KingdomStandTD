using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities
{
    public class Layers
    {
        public static readonly int Triggers = LayerMask.NameToLayer("Triggers");
        public static readonly LayerMask TriggersMask = 1 << Triggers;
        public static readonly LayerMask ExcludeTriggersMask = ~(1  << Triggers);

        public static readonly int Characters = LayerMask.NameToLayer("Characters");
        public static readonly LayerMask CharacterMask = 1 << Characters;

        public static readonly int Enviroment = LayerMask.NameToLayer("Enviroment");
        public static readonly LayerMask EnviromentMask = 1 << Enviroment;

        public static readonly int DeathZone = LayerMask.NameToLayer("DeathZone");
        public static readonly LayerMask DeathZoneMask = 1 << DeathZone;
    }
}
