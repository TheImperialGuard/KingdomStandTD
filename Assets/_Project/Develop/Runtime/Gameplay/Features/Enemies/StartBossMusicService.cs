using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class StartBossMusicService : IDisposable
    {
        private readonly MusicSwitcherService _musicSwitcherService;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public StartBossMusicService(MusicSwitcherService musicSwitcherService, EntitiesLifeContext entitiesLifeContext)
        {
            _musicSwitcherService = musicSwitcherService;
            _entitiesLifeContext = entitiesLifeContext;

            _entitiesLifeContext.Added += OnEntityAdded;
        }

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
        }

        private void OnEntityAdded(Entity entity)
        {
            if (entity.TryGetComponent(out IsBoss isBoss))
                _musicSwitcherService.SwitchFor(MusicContexts.GameplayBossfight);
        }
    }
}
