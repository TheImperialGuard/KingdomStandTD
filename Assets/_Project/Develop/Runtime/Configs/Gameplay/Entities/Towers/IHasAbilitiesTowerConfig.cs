using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    public interface IHasAbilitiesTowerConfig
    {
        IReadOnlyList<AbilityConfig> FirstAbilityGroup { get; }
        IReadOnlyList<AbilityConfig> SecondAbilityGroup { get; }
    }
}
