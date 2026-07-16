using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class PlayerInteractsService : IDisposable
    {
        private readonly RayShooterService _rayShooterService;

        private IDisposable _rayShooterDisposable;

        public PlayerInteractsService(RayShooterService rayShooterService)
        {
            _rayShooterService = rayShooterService;
        }

        public void Enable()
        {
            _rayShooterDisposable = _rayShooterService.LastHitInfo.Subscribe(OnRayShooted);
        }

        public void Disable()
        {
            _rayShooterDisposable.Dispose();
        }

        public void Dispose()
        {
            Disable();
        }

        private void OnRayShooted(RaycastHit oldHit, RaycastHit newHit)
        {
            if (newHit.collider.gameObject.TryGetComponent(out MonoEntity monoEntity) == false)
                return;

            if (monoEntity.LinkedEntity.TryGetComponent(out IsInteractable interactable))
            {
                if (monoEntity.LinkedEntity.TryGetComponent(out InteractRequest interactRequest))
                    interactRequest.Value.Invoke();
            }
        }
    }
}
