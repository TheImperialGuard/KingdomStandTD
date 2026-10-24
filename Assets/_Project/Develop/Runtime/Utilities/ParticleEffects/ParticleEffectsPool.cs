using System;
using System.Collections.Generic;
using CartoonFX;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.ParticleEffects
{
    // Переиспользует эффекты частиц вместо Instantiate на каждое событие.
    // Эффект возвращается в пул, когда у него не осталось живых частиц во всей иерархии и затих звук.
    // Самоуничтожение эффекта (stopAction у систем, clearBehavior у CFXR) у экземпляров пула выключено.
    public class ParticleEffectsPool : IDisposable
    {
        private const string RootName = "ParticleEffectsPool";

        private readonly Dictionary<ParticleSystem, Stack<PooledEffect>> _freeEffects = new();
        private readonly List<PooledEffect> _activeEffects = new();

        private readonly Transform _root;

        public ParticleEffectsPool()
        {
            _root = new GameObject(RootName).transform;
        }

        public ParticleSystem Spawn(ParticleSystem prefab, Vector3 position, Transform parent = null)
        {
            Transform targetParent = parent != null ? parent : _root;

            if (TryTakeFree(prefab, out PooledEffect effect) == false)
            {
                effect = Create(prefab, position, targetParent);

                _activeEffects.Add(effect);

                return effect.Instance;
            }

            Transform effectTransform = effect.Instance.transform;
            effectTransform.SetParent(targetParent, false);
            effectTransform.SetPositionAndRotation(position, Quaternion.identity);

            effect.Instance.gameObject.SetActive(true);

            if (effect.Instance.main.playOnAwake == true)
                effect.Instance.Play(true);

            _activeEffects.Add(effect);

            return effect.Instance;
        }

        public void Update()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                PooledEffect effect = _activeEffects[i];

                // Эффект с родителем уничтожается вместе с ним — например, вспышка у дула улучшенной пушки
                if (effect.Instance == null)
                {
                    RemoveActiveAt(i);
                    continue;
                }

                if (effect.IsFinished() == false)
                    continue;

                RemoveActiveAt(i);
                Release(effect);
            }
        }

        public void Dispose()
        {
            if (_root != null)
                GameObject.Destroy(_root.gameObject);

            _freeEffects.Clear();
            _activeEffects.Clear();
        }

        private PooledEffect Create(ParticleSystem prefab, Vector3 position, Transform parent)
        {
            ParticleSystem instance = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);

            foreach (ParticleSystem system in instance.GetComponentsInChildren<ParticleSystem>(true))
            {
                ParticleSystem.MainModule main = system.main;
                main.stopAction = ParticleSystemStopAction.None;
            }

            foreach (CFXR_Effect cfxrEffect in instance.GetComponentsInChildren<CFXR_Effect>(true))
                cfxrEffect.clearBehavior = CFXR_Effect.ClearBehavior.None;

            return new PooledEffect(prefab, instance, instance.GetComponentInChildren<AudioSource>(true));
        }

        private bool TryTakeFree(ParticleSystem prefab, out PooledEffect effect)
        {
            effect = null;

            if (_freeEffects.TryGetValue(prefab, out Stack<PooledEffect> freeEffects) == false)
                return false;

            return freeEffects.TryPop(out effect);
        }

        private void Release(PooledEffect effect)
        {
            effect.Instance.gameObject.SetActive(false);

            if (effect.Instance.transform.parent != _root)
                effect.Instance.transform.SetParent(_root, false);

            if (_freeEffects.TryGetValue(effect.Prefab, out Stack<PooledEffect> freeEffects) == false)
            {
                freeEffects = new Stack<PooledEffect>();
                _freeEffects.Add(effect.Prefab, freeEffects);
            }

            freeEffects.Push(effect);
        }

        // Порядок активных эффектов не важен: на место удалённого встаёт последний
        private void RemoveActiveAt(int index)
        {
            int lastIndex = _activeEffects.Count - 1;

            _activeEffects[index] = _activeEffects[lastIndex];
            _activeEffects.RemoveAt(lastIndex);
        }

        private class PooledEffect
        {
            public readonly ParticleSystem Prefab;
            public readonly ParticleSystem Instance;
            public readonly AudioSource AudioSource;

            public PooledEffect(ParticleSystem prefab, ParticleSystem instance, AudioSource audioSource)
            {
                Prefab = prefab;
                Instance = instance;
                AudioSource = audioSource;
            }

            public bool IsFinished()
            {
                if (Instance.IsAlive(true) == true)
                    return false;

                return AudioSource == null || AudioSource.isPlaying == false;
            }
        }
    }
}
