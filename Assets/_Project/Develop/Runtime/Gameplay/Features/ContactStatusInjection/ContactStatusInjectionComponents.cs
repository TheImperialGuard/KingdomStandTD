using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection
{
    public class BodyContactInjectingStatuses : IEntityComponent
    {
        public List<StatusesTypes> Value;
    }
}
