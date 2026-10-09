using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WarCryFeature
{
    public class WarCryTargetKinds : IEntityComponent
    {
        public List<EnemyKinds> Value;
    }
}
