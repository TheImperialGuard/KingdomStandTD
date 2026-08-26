using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Sensors
{
    public class AnotherTeamTouchDetectorSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _source;

        private Buffer<Entity> _contacts;

        private ReactiveVariable<bool> _isTouchAnotherTeam;

        public void OnInit(Entity entity)
        {
            _source = entity;
            _contacts = entity.ContactEntitiesBuffer;
            _isTouchAnotherTeam = entity.IsTouchAnotherTeam;
        }

        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                Entity contact = _contacts.Items[i];

                if (EntitiesHelper.IsSameTeam(_source, contact) == false)
                {
                    _isTouchAnotherTeam.Value = true;
                    return;
                }
            }

            _isTouchAnotherTeam.Value = false;
        }
    }
}
