using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Player
{
    public class DealDamageToPlayerService : IDisposable
    {
        private readonly PlayerHealth _playerHealth;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public DealDamageToPlayerService(PlayerHealth playerHealth, EntitiesLifeContext entitiesLifeContext)
        {
            _playerHealth = playerHealth;
            _entitiesLifeContext = entitiesLifeContext;

            _entitiesLifeContext.Released += OnEntityReleased;
        }

        private void OnEntityReleased(Entity entity)
        {
            if (entity.TryGetIsPathFinished(out ReactiveVariable<bool> isPathFinished) == false
                || isPathFinished.Value == false)
                return;

            if (entity.TryGetDamageOnFinishPath(out ReactiveVariable<int> damage))
                _playerHealth.TakeDamage(damage.Value);
        }

        public void Dispose()
        {
            _entitiesLifeContext.Released -= OnEntityReleased;
        }
    }
}
