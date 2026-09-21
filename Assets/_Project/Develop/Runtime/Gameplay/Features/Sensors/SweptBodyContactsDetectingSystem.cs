using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Sensors
{
    public class SweptBodyContactsDetectingSystem : IInitializableSystem, IUpdatableSystem
    {
        private const int SWEEP_HITS_BUFFER_SIZE = 64;
        private const float MIN_SWEEP_DISTANCE = 0.0001f;

        private readonly RaycastHit[] _sweepHits = new RaycastHit[SWEEP_HITS_BUFFER_SIZE];

        private Buffer<Collider> _contacts;
        private LayerMask _mask;

        private CapsuleCollider _body;

        private Vector3 _previousPosition;

        public void OnInit(Entity entity)
        {
            _contacts = entity.ContactCollidersBuffer;
            _mask = entity.ContactsDetectingMask;

            _body = entity.BodyCollider;

            _previousPosition = _body.bounds.center;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector3 currentPosition = _body.bounds.center;

            DetectContactsAtCurrentPosition();
            DetectContactsOnPathFrom(_previousPosition, currentPosition);

            RemoveSelfFromContacts();

            _previousPosition = currentPosition;
        }

        private void DetectContactsAtCurrentPosition()
        {
            _contacts.Count = Physics.OverlapCapsuleNonAlloc(
                _body.bounds.min,
                _body.bounds.max,
                _body.radius,
                _contacts.Items,
                _mask,
                QueryTriggerInteraction.Ignore);
        }

        private void DetectContactsOnPathFrom(Vector3 previousPosition, Vector3 currentPosition)
        {
            Vector3 path = currentPosition - previousPosition;

            float distance = path.magnitude;

            if (distance < MIN_SWEEP_DISTANCE)
                return;

            int hitsCount = Physics.SphereCastNonAlloc(
                previousPosition,
                _body.radius,
                path / distance,
                _sweepHits,
                distance,
                _mask,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitsCount; i++)
                AddContact(_sweepHits[i].collider);
        }

        private void AddContact(Collider collider)
        {
            if (collider == null)
                return;

            if (_contacts.Count == _contacts.Items.Length)
                return;

            if (HasContact(collider))
                return;

            _contacts.Items[_contacts.Count] = collider;
            _contacts.Count++;
        }

        private bool HasContact(Collider collider)
        {
            for (int i = 0; i < _contacts.Count; i++)
                if (_contacts.Items[i] == collider)
                    return true;

            return false;
        }

        private void RemoveSelfFromContacts()
        {
            int indexToRemove = -1;

            for (int i = 0; i < _contacts.Count; i++)
            {
                if (_contacts.Items[i] == _body)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove >= 0)
            {
                for (int i = indexToRemove; i < _contacts.Count - 1; i++)
                {
                    _contacts.Items[i] = _contacts.Items[i + 1];
                }

                _contacts.Count--;
            }
        }
    }
}
