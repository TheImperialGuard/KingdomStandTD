using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class InteractSystem : IInitializableSystem, IDisposableSystem
    {

        private ReactiveEvent _interactRequest;
        private ReactiveEvent _interactEvent;

        private ICompositeCondition _canInteract;

        private ReactiveVariable<IInteractAction> _interactiveAction;

        private IDisposable _requestDisposable;

        public void OnInit(Entity entity)
        {
            _interactRequest = entity.InteractRequest;
            _interactEvent = entity.InteractEvent;

            _canInteract = entity.CanInteract;

            _interactiveAction = entity.InteractiveAction;

            _requestDisposable = _interactRequest.Subscribe(OnInteractRequest);
        }

        public void OnDispose() => _requestDisposable?.Dispose();

        private void OnInteractRequest()
        {
            if (_canInteract.Evaluate() == false)
                return;

            _interactiveAction.Value.Do();

            _interactEvent.Invoke();
        }
    }
}
