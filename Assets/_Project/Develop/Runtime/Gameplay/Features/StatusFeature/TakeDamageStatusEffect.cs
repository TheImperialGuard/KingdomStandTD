using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class TakeDamageStatusEffect : IStatusEffect
    {
        private readonly ReactiveVariable<float> _damage;

        public TakeDamageStatusEffect(ReactiveVariable<float> damage)
        {
            _damage = damage;
        }

        public void Apply(Entity entity)
        {
            if (entity.TryGetTakeDamageRequest(out ReactiveEvent<float> takeDamageRequest) == false)
                return;

            takeDamageRequest.Invoke(_damage.Value);
        }
    }
}
