using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackCancelSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _inAttackProcess;

        private ReactiveEvent _attackCancelEvent;

        private ICompositeCondition _mustCancelAttack;

        public void OnInit(Entity entity)
        {
            _inAttackProcess = entity.InAttackProcess;
            _attackCancelEvent = entity.AttackCancelEvent;
            _mustCancelAttack = entity.MustCancelAttack;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inAttackProcess.Value == false)
                return;

            if (_mustCancelAttack.Evaluate())
            {
                Debug.Log("Процесс атаки прерван");
                _inAttackProcess.Value = false;
                _attackCancelEvent.Invoke();
            }
        }
    }
}
