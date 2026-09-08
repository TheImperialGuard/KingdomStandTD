using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class CreateFragmentsOnDeathSystem : IInitializableSystem, IDisposableSystem
    {
        public const float UpwardFragmentForce = 3f;
        public const float HorizontalFragmentForce = 1f;

        private readonly ProjectilesFactory _projectilesFactory;

        private ReactiveVariable<float> _fragmentsDamage;
        private ReactiveVariable<float> _fragmentsCount;

        private Entity _source;
        private Transform _deathPoint;

        private ReactiveVariable<bool> _isDead;

        private IDisposable _isDeadDisposable;

        private Vector3 FragmentsSpawnDirection => Vector3.up;

        public CreateFragmentsOnDeathSystem(ProjectilesFactory projectilesFactory)
        {
            _projectilesFactory = projectilesFactory;
        }

        public void OnInit(Entity entity)
        {
            _fragmentsDamage = entity.FragmentsDamage;
            _fragmentsCount = entity.FragmentsCount;

            _source = entity;
            _deathPoint = entity.Transform;

            _isDead = entity.IsDead;

            _isDeadDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        public void OnDispose()
        {
            _isDeadDisposable.Dispose();
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead == true)
                CreateFragments();
        }

        private void CreateFragments()
        {
            Vector3 spawnPoint = _deathPoint.position;

            spawnPoint.y += 0.5f;

            float angleStep = 360f / _fragmentsCount.Value;

            for (int i = 0; i < _fragmentsCount.Value; i++)
            {
                float angle = i * angleStep;

                float angleRad = angle * Mathf.Deg2Rad;
                Vector3 fragmentDirection = new Vector3(
                    Mathf.Cos(angleRad),
                    0,
                    Mathf.Sin(angleRad)
                ).normalized;

                Entity fragment = _projectilesFactory.Create(
                ProjectilesTypes.Cannonball,
                spawnPoint,
                FragmentsSpawnDirection,
                _source.Owner.Value);

                fragment.InstantAttackDamage.Value = _fragmentsDamage.Value;

                Launch(fragment, fragmentDirection);
            }
        }

        private void Launch(Entity fragment, Vector3 direction)
        {
            Vector3 horizontalDirection = direction.normalized;

            horizontalDirection.y = 0;

            if (horizontalDirection == Vector3.zero)
                horizontalDirection = fragment.Transform.forward;

            Vector3 launchVelocity = Vector3.up * UpwardFragmentForce + horizontalDirection * HorizontalFragmentForce;

            fragment.Rigidbody.AddForce(launchVelocity, ForceMode.VelocityChange);
        }
    }
}
