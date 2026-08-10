using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    public interface ITargetSelector
    {
        Entity SelectTargetFrom(IEnumerable<Entity> targets);
        List<Entity> SelectMultipleTargetsFrom(IEnumerable<Entity> targets, int count);
    }
}
