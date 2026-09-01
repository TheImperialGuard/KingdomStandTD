using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class AreaEffectRadius : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
