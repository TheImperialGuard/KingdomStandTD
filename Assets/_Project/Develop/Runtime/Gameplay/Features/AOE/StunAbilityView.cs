using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.SimpleMono;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    public class StunAbilityView : EntityView
    {
        [SerializeField] private ParticleSystem _effectPrefab;

        private Entity _entity;

        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<float> _stunDuration;
        private ReactiveVariable<float> _areaEffectRadius;

        private ReactiveEvent _abilityDelayEndEvent;

        private bool _isAbilityAdded;

        private IDisposable _abilityDelayEndDisposable;

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _abilityDelayEndDisposable?.Dispose();
            _entity.Abilities.Added -= OnAbilityAdded;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;

            _entity.Abilities.Added += OnAbilityAdded;
        }

        private void OnAbilityAdded(Ability ability)
        {
            if (ability is StunAbility == false)
                return;

            _currentTarget = _entity.CurrentTarget;
            _stunDuration = _entity.StunDuration;
            _areaEffectRadius = _entity.AreaEffectRadius;
            _abilityDelayEndEvent = _entity.SecondAbilityDelayEndEvent;
            _abilityDelayEndDisposable = _abilityDelayEndEvent.Subscribe(OnDelayEnd);

            _isAbilityAdded = true;
        }

        private void OnDelayEnd() => CreateEffect();

        private void CreateEffect()
        {
            ParticleSystem effect = Instantiate(_effectPrefab, _currentTarget.Value.Transform.position, Quaternion.identity);

            ParticleEffectTimeScaler timeScaler = effect.GetComponent<ParticleEffectTimeScaler>();

            timeScaler.PlayForDuration(_stunDuration.Value);
        }

        private void OnDrawGizmos()
        {
            if (_isAbilityAdded == false) 
                return;

            //DrawCircleXZ(_currentTarget.Value.Transform.position, _areaEffectRadius.Value, Color.blue, 30);
        }

        private static void DrawCircleXZ(Vector3 center, float radius, Color color, int segments)
        {
            if (radius <= 0f || segments < 3)
                return;

            Gizmos.color = color;

            float angleStep = Mathf.PI * 2f / segments;

            Vector3 previousPoint = center + new Vector3(radius, 0f, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i;

                Vector3 nextPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius);

                Gizmos.DrawLine(previousPoint, nextPoint);

                previousPoint = nextPoint;
            }
        }
    }
}
