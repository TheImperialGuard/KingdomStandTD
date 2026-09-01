using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.SimpleMono
{
    public class ParticleEffectTimeScaler : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _targetDuration = 1f;
        [SerializeField] private float _additionalTime = 0f;

        private ParticleSystem _rootSystem;
        private ParticleSystem[] _systems;

        private Dictionary<ParticleSystem, float> _baseSimulationSpeeds = new();

        private void Awake()
        {
            CacheParticleSystems();
        }

        private void OnEnable()
        {
            if (_systems == null || _systems.Length == 0)
                CacheParticleSystems();

            SetDuration(_targetDuration);
        }

        public void SetDuration(float newTargetDuration)
        {
            _targetDuration = Mathf.Max(0.01f, newTargetDuration);

            float originalDuration = CalculateEffectDuration();

            if (originalDuration <= 0f)
            {
                Debug.LogWarning(
                    $"[{name}] Не удалось определить длительность particle effect.",
                    this);

                return;
            }

            float scale = originalDuration / _targetDuration;
            SetTimeScale(scale);
        }

        public void SetTimeScale(float scale)
        {
            scale = Mathf.Max(0.001f, scale);

            foreach (ParticleSystem system in _systems)
            {
                if (system == null)
                    continue;

                var main = system.main;
                main.simulationSpeed = _baseSimulationSpeeds[system] * scale - _additionalTime;
            }
        }

        public void ResetTimeScale()
        {
            foreach (ParticleSystem system in _systems)
            {
                if (system == null)
                    continue;

                var main = system.main;
                main.simulationSpeed = _baseSimulationSpeeds[system];
            }
        }

        public void PlayForDuration(float duration)
        {
            SetDuration(duration);

            ParticleSystem rootSystem = GetComponent<ParticleSystem>();
            rootSystem.Play(withChildren: true);
        }

        private void CacheParticleSystems()
        {
            _systems = GetComponentsInChildren<ParticleSystem>(true);

            _baseSimulationSpeeds.Clear();

            foreach (ParticleSystem system in _systems)
            {
                _baseSimulationSpeeds[system] = system.main.simulationSpeed;
            }
        }

        private float CalculateEffectDuration()
        {
            float longestDuration = 0f;

            foreach (ParticleSystem system in _systems)
            {
                if (system == null)
                    continue;

                var main = system.main;

                if (main.loop)
                {
                    Debug.LogWarning(
                        $"[{system.name}] Включён Looping: у такого эффекта нет конечной длительности.",
                        system);

                    continue;
                }

                float particleLifetime = GetMaxLifetime(main.startLifetime);

                float systemDuration =
                    main.startDelay.constantMax +
                    main.duration +
                    particleLifetime;

                longestDuration = Mathf.Max(longestDuration, systemDuration);
            }

            return longestDuration;
        }

        private static float GetMaxLifetime(ParticleSystem.MinMaxCurve lifetime)
        {
            return lifetime.mode switch
            {
                ParticleSystemCurveMode.Constant =>
                    lifetime.constant,

                ParticleSystemCurveMode.TwoConstants =>
                    lifetime.constantMax,

                ParticleSystemCurveMode.Curve =>
                    lifetime.curveMultiplier,

                ParticleSystemCurveMode.TwoCurves =>
                    lifetime.curveMultiplier,

                _ => lifetime.constantMax
            };
        }
    }
}
