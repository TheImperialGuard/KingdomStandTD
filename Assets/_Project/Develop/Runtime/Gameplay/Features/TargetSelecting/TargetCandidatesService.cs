using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting
{
    // Сущности, которые селекторы вообще могут выбрать целью: состоят в команде и принимают урон.
    // Порядок — как в EntitiesLifeContext, поэтому выбор цели совпадает с обходом всех сущностей.
    public class TargetCandidatesService : IDisposable
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private readonly List<Entity> _candidates = new();

        public TargetCandidatesService(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;

            foreach (Entity entity in _entitiesLifeContext.Entities)
                OnEntityAdded(entity);

            _entitiesLifeContext.Added += OnEntityAdded;
            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public IReadOnlyList<Entity> Candidates => _candidates;

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
            _entitiesLifeContext.Released -= OnEntityReleased;

            _candidates.Clear();
        }

        private bool IsCandidate(Entity entity) => entity.HasComponent<Team>() && entity.HasComponent<TakeDamageRequest>();

        private void OnEntityAdded(Entity entity)
        {
            if (IsCandidate(entity))
                _candidates.Add(entity);
        }

        private void OnEntityReleased(Entity entity) => _candidates.Remove(entity);
    }
}
