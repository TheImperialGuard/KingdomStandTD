using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public interface ITargetSelector
    {
        Entity SelectTargetFrom(IReadOnlyList<Entity> targets);
        List<Entity> SelectMultipleTargetsFrom(IReadOnlyList<Entity> targets, int count);
    }
}
