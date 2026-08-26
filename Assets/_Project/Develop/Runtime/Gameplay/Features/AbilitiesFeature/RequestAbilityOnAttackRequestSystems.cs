using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    public class RequestFirstAbilityOnAttackRequestSystem : IInitializableSystem, IDisposable
    {
        private ReactiveEvent _startAttackRequest;
        private ReactiveEvent _startAbilityRequest;

        private IDisposable _startAttackRequestDisposable;

        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.StartAttackRequest;
            _startAbilityRequest = entity.StartFirstAbilityRequest;

            _startAttackRequestDisposable = _startAttackRequest.Subscribe(OnStartAttackRequested);
        }

        public void Dispose()
        {
            _startAttackRequestDisposable.Dispose();
        }

        private void OnStartAttackRequested()
        {
            _startAbilityRequest.Invoke();
        }
    }

    public class RequestSecondAbilityOnAttackRequestSystem : IInitializableSystem, IDisposable
    {
        private ReactiveEvent _startAttackRequest;
        private ReactiveEvent _startAbilityRequest;

        private IDisposable _startAttackRequestDisposable;

        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.StartAttackRequest;
            _startAbilityRequest = entity.StartSecondAbilityRequest;

            _startAttackRequestDisposable = _startAttackRequest.Subscribe(OnStartAttackRequested);
        }

        public void Dispose()
        {
            _startAttackRequestDisposable.Dispose();
        }

        private void OnStartAttackRequested() => _startAbilityRequest.Invoke();
    }
}
