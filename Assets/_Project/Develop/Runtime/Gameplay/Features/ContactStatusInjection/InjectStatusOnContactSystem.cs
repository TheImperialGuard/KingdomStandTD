using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection
{
    public class InjectStatusOnContactSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly StatusesFactory _statusesFactory;

        private Entity _source;

        private Buffer<Entity> _contacts;

        private List<StatusesTypes> _statutesForInject;

        private List<Entity> _processedEntities;

        public InjectStatusOnContactSystem(StatusesFactory statusesFactory)
        {
            _statusesFactory = statusesFactory;
        }

        public void OnInit(Entity entity)
        {
            _source = entity;

            _contacts = entity.ContactEntitiesBuffer;

            _statutesForInject = entity.BodyContactInjectingStatuses;

            _processedEntities = new List<Entity>(_contacts.Items.Length);
        }

        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                Entity contactEntity = _contacts.Items[i];

                if (_processedEntities.Contains(contactEntity) == false)
                {
                    _processedEntities.Add(contactEntity);

                    if (EntitiesHelper.IsSameTeam(_source, contactEntity) == false)
                    {
                        InjectStatusesTo(contactEntity);
                    }
                }
            }

            for (int i = _processedEntities.Count - 1; i >= 0; i--)
                if (ContainInContacts(_processedEntities[i]) == false)
                    _processedEntities.RemoveAt(i);
        }

        public bool ContainInContacts(Entity entity)
        {
            for (int i = 0; i < _contacts.Count; i++)
                if (_contacts.Items[i] == entity)
                    return true;

            return false;
        }

        private void InjectStatusesTo(Entity contactEntity)
        {
            foreach (StatusesTypes statusType in _statutesForInject)
            {
                Status status = _statusesFactory.CreateFor(
                    contactEntity, 
                    statusType, 
                    _source, 
                    _source.LastingDamageInitialTime.Value,
                    _source.LastingDamageInterval.Value);

                EntitiesHelper.TryInjectStatusTo(contactEntity, status);
            }
        }
    }
}
