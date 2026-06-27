using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IDisposable
    {
        private ReactiveEvent _startAttackRequest;
        private ReactiveEvent _startAttackEvent;

        private ICompositeCondition _canStartAttack;

        private IDisposable _attackRequestDisposable;

        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.StartAttackRequest;
            _startAttackEvent = entity.StartAttackEvent;

            _canStartAttack = entity.CanStartAttack;

            _attackRequestDisposable = _startAttackRequest.Subscribe(OnAttackRequest);
        }

        public void Dispose()
        {
            _attackRequestDisposable.Dispose();
        }

        private void OnAttackRequest()
        {
            if (_canStartAttack.Evaluate())
            {
                _startAttackEvent.Invoke();
                Debug.Log("Старт атаки");
            }
            else
            {
                Debug.Log("Не могу атаковать");
            }
        }
    }
}
